// Based on code (c) Copyright HutongGames, LLC 2010-2011. All rights reserved.
// by MaDDoX (@brenoazevedo on Twitter)

using UnityEngine;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Detect collisions with a Tagged trigger and Broadcasts an event\nUse this action to Broadcast an event when colliding with a particular Tag.")]
    public class TriggerBroadcastEvent : FsmStateAction
	{
		public TriggerType trigger;
		[UIHint(UIHint.Tag)]
		public FsmString collideTag;
        [UIHint(UIHint.Variable)]
        public FsmGameObject storeCollider;
        public FsmString broadcastEvent;
        [Tooltip("Optionally specify a game object to broadcast the event to all FSMs on that game object.")]
        public FsmGameObject gameObject;
        [Tooltip("Broadcast to all FSMs on the game object's children.")]
        public FsmBool sendToChildren;
        public FsmBool excludeSelf;

		public override void Reset()
		{
			trigger = TriggerType.OnTriggerEnter;
			collideTag = "Untagged";
            broadcastEvent = null;
			storeCollider = null;
		}

        public override void OnEnter()
        {
            if (string.IsNullOrEmpty(broadcastEvent.Value)) Finish();
        }

		public override void DoTriggerEnter(Collider other)
		{
			if (trigger == TriggerType.OnTriggerEnter)
			{
				if (other.gameObject.tag == collideTag.Value)
				{
                    storeCollider.Value = other.gameObject;
                    DoBroadcastEvent();
				}
			}
		}

		public override void DoTriggerStay(Collider other)
		{
			if (trigger == TriggerType.OnTriggerStay)
			{
				if (other.gameObject.tag == collideTag.Value)
				{
                    storeCollider.Value = other.gameObject;
                    DoBroadcastEvent();
				}
			}
		}

		public override void DoTriggerExit(Collider other)
		{
			if (trigger == TriggerType.OnTriggerExit)
			{
				if (other.gameObject.tag == collideTag.Value)
				{
                    storeCollider.Value = other.gameObject;
                    DoBroadcastEvent();
				}
			}
		}

        void DoBroadcastEvent()
        {
            if (gameObject.Value != null)
            {
                BroadcastToGameObject(gameObject.Value);
            }
            else
            {
                BroadcastToAll();
            }
        }

        void BroadcastToAll()
        {
            // copy the list in case broadcast event changes Fsm.FsmList

            var fsmList = new List<Fsm>(Fsm.FsmList);

            foreach (var fsm in fsmList)
            {
                if (excludeSelf.Value && fsm == Fsm)
                {
                    continue;
                }

                fsm.Event(broadcastEvent.Value);
            }
        }

        void BroadcastToGameObject(GameObject go)
        {
            if (go == null) return;

            var fsmComponents = go.GetComponents<PlayMakerFSM>();

            foreach (var fsmComponent in fsmComponents)
            {
                fsmComponent.Fsm.Event(broadcastEvent.Value);
            }

            if (sendToChildren.Value)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    BroadcastToGameObject(go.transform.GetChild(i).gameObject);
                }
            }
        }

		public override string ErrorCheck()
		{
			return ActionHelpers.CheckOwnerPhysicsSetup(Owner);
		}


	}
}