using System;
using UnityEngine;

namespace ES3Types
{
	[ES3PropertiesAttribute("animationSequence", "objectToBeAnimated", "loopable", "animationFinished", "pause", "endDistanceTolerance", "isAnimationRunning", "testing", "enabled")]
	public class ES3Type_LinearObjectMovement : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3Type_LinearObjectMovement() : base(typeof(LinearObjectMovement))
		{
			Instance = this;
		}

		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (LinearObjectMovement)obj;
			
			writer.WriteProperty("animationSequence", instance.animationSequence);
			writer.WritePropertyByRef("objectToBeAnimated", instance.objectToBeAnimated);
			writer.WriteProperty("loopable", instance.loopable, ES3Type_bool.Instance);
			writer.WriteProperty("animationFinished", instance.animationFinished, ES3Type_bool.Instance);
			writer.WriteProperty("pause", instance.pause, ES3Type_bool.Instance);
			writer.WriteProperty("endDistanceTolerance", instance.endDistanceTolerance, ES3Type_float.Instance);
			writer.WriteProperty("isAnimationRunning", instance.isAnimationRunning, ES3Type_bool.Instance);
			writer.WriteProperty("testing", instance.testing, ES3Type_bool.Instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (LinearObjectMovement)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "animationSequence":
						instance.animationSequence = reader.Read<MovementInfo[]>();
						break;
					case "objectToBeAnimated":
						instance.objectToBeAnimated = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "loopable":
						instance.loopable = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "animationFinished":
						instance.animationFinished = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "pause":
						instance.pause = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "endDistanceTolerance":
						instance.endDistanceTolerance = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "isAnimationRunning":
						instance.isAnimationRunning = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "testing":
						instance.testing = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}

	public class ES3Type_LinearObjectMovementArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3Type_LinearObjectMovementArray() : base(typeof(LinearObjectMovement[]), ES3Type_LinearObjectMovement.Instance)
		{
			Instance = this;
		}
	}
}