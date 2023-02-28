using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ChatSample : MonoBehaviour {

    [SerializeField] TurnBasedNetwork.NetworkManager network;

    int nowTurn;

    [SerializeField] Text inText;
    [SerializeField] Text outText;
    [SerializeField] Text myText;
    [SerializeField] Button sendButton;

    bool isPressed;


    void Start() {
        nowTurn = 0;
        isPressed = false;
        sendButton.onClick.AddListener(() => isPressed = true);
        StartCoroutine(MainCoroutine());
    }


    //メインコルーチン
    IEnumerator MainCoroutine() {

        //ネットワークが接続されるまで待機
        sendButton.interactable = false;
        yield return network.ConnectNetwork();
        sendButton.interactable = true;
        

        while(true) {
            //自分のターンの時
            if(nowTurn == network.GetMyId()) {
                sendButton.interactable = true;
                isPressed = false;
                //メッセージを送る準備ができるまで待機する
                yield return new WaitUntil(() => isPressed);
                myText.text = outText.text;
                //メッセージを送信する
                network.SendMessage(new ChatNetworkMassage(myText.text));
            }
            //相手のターンの時
            else {
                sendButton.interactable = false;
                //メッセージを受け取るまで待機する
                IEnumerator c = network.Wait(nowTurn);
                yield return c;
                var message = (TurnBasedNetwork.NetworkMessage)c.Current;
                //受け取ったメッセージから値を抽出する
                inText.text = (string)message.GetValue();
            }
            //ターン交代
            nowTurn = (nowTurn + 1) % 2;
        }
    }  

}
