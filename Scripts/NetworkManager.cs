using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


namespace TurnBasedNetwork {

    public class NetworkManager : MonoBehaviourPunCallbacks {

        bool isConnected;

        int myPlayerId;
        List<Queue<NetworkMessage>> queues;

        [SerializeField] PhotonView view;

        [SerializeField] int maxPlayerNum;


        public IEnumerator ConnectNetwork() {
            
            if(isConnected) yield break;
            Debug.Log("connecting...");
            PhotonNetwork.ConnectUsingSettings();
            while(!isConnected) {
                Debug.Log("wait for other player...");
                yield return null;
            }
            Debug.Log("connected");
        }

        public override void OnConnectedToMaster() {
            PhotonNetwork.JoinRandomRoom();
        }

        public override void OnJoinRandomFailed(short returnCode, string message) {
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = (byte)maxPlayerNum;

            PhotonNetwork.CreateRoom(null, roomOptions);
        }

        public override void OnJoinedRoom() {
            myPlayerId = PhotonNetwork.CurrentRoom.PlayerCount - 1;
            StartCoroutine(WaitForOtherPlayer());
        }

        IEnumerator WaitForOtherPlayer() {
            while(PhotonNetwork.CurrentRoom.PlayerCount < maxPlayerNum) {
                yield return null;
            }
            PhotonNetwork.CurrentRoom.IsOpen = false;
            Setup();
        }

        void Setup() {
            isConnected = true;
            queues = new List<Queue<NetworkMessage>>();
            foreach(var player in PhotonNetwork.PlayerList) {
                queues.Add(new Queue<NetworkMessage>());
            }
        }

        public bool CheckConnected() {
            return isConnected;
        }

        public int GetMyId() {
            return myPlayerId;
        }

        public int GetPlayerNum() {
            return queues.Count;
        }


        public void SendMessage(NetworkMessage message) {
            if(!isConnected) return;
            view.RPC(nameof(ReceiveMessage), RpcTarget.Others, message.Encode(), myPlayerId);
        }

        [PunRPC]
        void ReceiveMessage(string code, int senderId) {
            var message = new NetworkMessage();
            message.Decode(code);
            queues[senderId].Enqueue(message);
        }

        public IEnumerator Wait(int id) {
            if(!isConnected) yield break;
            NetworkMessage ret = null;
            while(queues[id].Count == 0) {
                yield return null;
            }
            var message = queues[id].Dequeue();
            ret = message;
            yield return ret;
        }

    }

}
