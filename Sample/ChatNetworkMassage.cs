using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ChatNetworkMassage : TurnBasedNetwork.NetworkMessage {

    string text;

    public ChatNetworkMassage(string text) {
        this.text = text;
    }

    public override string Encode() {
        return text;
    }

    public override void Decode(string code) {
        Debug.Log(code);
        text = code;
    }

    public override System.Object GetValue() {
        return (System.Object)text;
    }

}
