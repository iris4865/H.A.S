using HatchlingNet;
using Header;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Server
{
    public class ObjectNumbering : IResponse
    {
        public IGameUser User { get; set; }
        string objectTag;
        MyVector3 vec;
        int gameUserID;
        string userID;
        Dictionary<int, string> objectList;


        public void Initialize(IGameUser user, Packet msg)
        {
            User = user;
            objectTag = msg.PopString();
            vec = msg.PopVector();
        }

        public void Process()
        {
            lock (GameUser.objNumberingPool)
            {
                User.GameUserID = GameUser.objNumberingPool.Pop();
                gameUserID = User.GameUserID;
            }
            userID = User.UserID;
            objectList = GameUser.objList;

            Trace.WriteLine($"태그 : {objectTag} 위치 x : {vec.x} y : {vec.y} z : {vec.z} remoteID : {gameUserID}");
        }

        public void Send()
        {
            Packet response = PacketBufferManager.Instance.Pop((short)PROTOCOL.ObjectNumberingAck);

            response.Push(objectTag);
            response.Push(vec);
            response.Push(gameUserID);

            //만약 이 메세지를 받은 클라의 userID와 같으면 그건 그사람이 주체적으로 만든거고
            //그 플레이어 조종하려고 만든거일 확률이 높음
            response.Push(userID);
            User.SendAll(response);


            foreach (var iter in objectList)
            {
                response = PacketBufferManager.Instance.Pop((short)PROTOCOL.ObjectNumberingAck);

                MyVector3 otherPlayerPos; otherPlayerPos.x = 0f; otherPlayerPos.y = 10f; otherPlayerPos.z = 0f;
                response.Push(objectTag);
                response.Push(otherPlayerPos);
                response.Push(iter.Key);
                response.Push("None");
                User.SendTo(User.UserID, response);
            }
            objectList.Add(gameUserID, objectTag);
        }
    }
}
