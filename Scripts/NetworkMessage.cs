using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedNetwork {

    public class NetworkMessage {

        public virtual string Encode() {return null;}

        public virtual void Decode(string code) {}

    }

}
