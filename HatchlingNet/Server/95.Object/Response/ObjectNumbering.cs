using HatchlingNet;
using Header;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Server
{
    public class ObjectNumbering : IResponse
    {
        SEND_TYPE sendType;
        string objectTag;
        MyVector3 vec;
        int gameUserID;
        string userID;
        Dictionary<int, string> objectList;


        public void Initialize(Packet msg)
        {
            sendType = (SEND_TYPE)msg.PopSendType();
            objectTag = msg.PopString();
            vec = msg.PopVector();
        }

        public void Process(GameUser user)
        {
            lock (GameUser.objNumberingPool)
            {
                user.GameUserID = GameUser.objNumberingPool.Pop();
                gameUserID = user.GameUserID;
            }
            userID = user.UserID;
            objectList = GameUser.objList;

            Trace.WriteLine($"태그 : {objectTag} 위치 x : {vec.x} y : {vec.y} z : {vec.z} remoteID : {user.GameUserID}");
        }

        public void Send(Action<Packet> send)
        {
            Packet response = PacketBufferManager.Instance.Pop((short)PROTOCOL.ObjectNumberingAck, (short)SEND_TYPE.BroadcastWithMe);

            response.Push(objectTag);
            response.Push(vec);
            response.Push(gameUserID);

            //만약 이 메세지를 받은 클라의 userID와 같으면 그건 그사람이 주체적으로 만든거고
            //그 플레이어 조종하려고 만든거일 확률이 높음
            response.Push(userID);
            send(response);


            foreach (var iter in objectList)
            {
                response = PacketBufferManager.Instance.Pop((short)PROTOCOL.ObjectNumberingAck, (short)SEND_TYPE.Single);

                MyVector3 otherPlayerPos; otherPlayerPos.x = 0f; otherPlayerPos.y = 10f; otherPlayerPos.z = 0f;
                response.Push(objectTag);
                response.Push(otherPlayerPos);
                response.Push(iter.Key);
                response.Push("None");
                send(response);
            }
            objectList.Add(gameUserID, objectTag);
        }
    }
}
