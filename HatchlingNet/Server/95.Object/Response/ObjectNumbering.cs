using HatchlingNet;
using Header;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Server
{
    public class ObjectNumbering : IResponse
    {
        public IGameUser Self { get; set; }
        string objectTag;
        MyVector3 vec;
        int gameUserID;
        string userID;
        Dictionary<int, string> objectList;


        public void Initialize(IGameUser user, Packet msg)
        {
            Self = user;
            objectTag = msg.PopString();
            vec = msg.PopVector();
        }

        public void Process()
        {
            lock (GameUser.objNumberingPool)
            {
                Self.GameUserID = GameUser.objNumberingPool.Pop();
                gameUserID = Self.GameUserID;
            }
            userID = Self.UserID;
            objectList = GameUser.objList;

            Trace.WriteLine($"태그 : {objectTag} 위치 x : {vec.x} y : {vec.y} z : {vec.z} remoteID : {gameUserID}");
        }

        public void Send()
        {
            Packet response = PacketBufferManager.Instance.Pop(PROTOCOL.ObjectNumberingAck);

            response.Push(objectTag);
            response.Push(vec);
            response.Push(gameUserID);
            response.Push(userID);
            Self.SendAll(response);


            foreach (var iter in objectList)
            {
                response = PacketBufferManager.Instance.Pop(PROTOCOL.ObjectNumberingAck);

                MyVector3 otherPlayerPos; otherPlayerPos.x = 0f; otherPlayerPos.y = 10f; otherPlayerPos.z = 0f;
                response.Push(objectTag);
                response.Push(otherPlayerPos);
                response.Push(iter.Key);
                response.Push("None");
                Self.SendTo(Self.UserID, response);
            }
            objectList.Add(gameUserID, objectTag);
        }
    }
}
