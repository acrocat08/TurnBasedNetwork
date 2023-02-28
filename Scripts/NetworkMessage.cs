using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedNetwork {

    public class NetworkMessage : ScriptableObject {

        public virtual string Encode() {return null;}

        public virtual void Decode(string code) {}
        public virtual System.Object GetValue() {return null;}

    }

}
