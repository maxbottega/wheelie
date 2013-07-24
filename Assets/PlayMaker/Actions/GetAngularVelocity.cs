using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Gets the Angular Velocity of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable. NOTE: The Game Object must have a Rigid Body.")]
	public class GetAngularVelocity : FsmStateAction {
		
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		public FsmOwnerDefault gameObject;
		[UIHint(UIHint.Variable)]
		public FsmVector3 vector;
		[UIHint(UIHint.Variable)]
		public FsmFloat x;
		[UIHint(UIHint.Variable)]
		public FsmFloat y;
		[UIHint(UIHint.Variable)]
		public FsmFloat z;
		public Space space;
		public bool everyFrame;
		
		public override void Reset()
		{
			gameObject = null;
			vector = null;
			x = null;
			y = null;
			z = null;
			space = Space.World;
			everyFrame = false;
		}
		
		public override void OnEnter()
		{
			DoGetAngularVelocity();
			
			if (!everyFrame)
				Finish();		
		}
		
		public override void OnUpdate()
		{
			DoGetAngularVelocity();
		}
		
		void DoGetAngularVelocity()
		{
			GameObject go = Fsm.GetOwnerDefaultTarget(gameObject);
			if (go == null) return;
			if (go.rigidbody == null) return;

			Vector3 angularVelocity = go.rigidbody.angularVelocity;

			if (space == Space.Self)
				angularVelocity = go.transform.InverseTransformDirection(angularVelocity);
			
			vector.Value = angularVelocity;
			x.Value = angularVelocity.x;
			y.Value = angularVelocity.y;
			z.Value = angularVelocity.z;
		}
	}
}