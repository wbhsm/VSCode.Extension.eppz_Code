﻿using System;
using System.Collections;
using System.Collections.Generic;
using Shape = EPPZ.Geometry.Polygon;


// Namespace declaration (curly brackets).
namespace Project
{


	// Using statements, built-in types, type lists.
	using User;
	using Record = List<Tuple<int, string, float, byte>>;


	[ExecuteInEditMode] 
	public class Game : MonoBehaviour
	{


		// Storage types, keywords, accessors, `this` and `base`.
		public static Colors shared { get; set; }
		void Awake()
		{
			shared = this;
			base.Initialize();
		}

		// Attributes, some literals, types, built-in types.
		const string version = "1.0";
		public int session = 0;
		bool isStarted = true;
		public Counter launchCounter;
		[UnityEngine.Serialization.FormerlySerializedAs("saveFile")]
		public string saveFileName = "save.json";

		// Accessors, methods (parenthesis).
		User.User user = null;
		public User.User GetUser() { return user; }
		public static User.Settings settings { get { return shared.user.settings; } }
		public static string UUID { get { return shared.user.UUID; } }

		// Accessors.
		public Features _features;
		public static Features features
		{
			get { return shared._features; }
			set { shared._features = value; }
		}

		
		void Start()
		{
			// Conditionals, operators.
			if (debugMessages) Debug.Log(name + ".Initialize()");
			int qualityIndex = (Screen.dpi > 250.0f) ? 1 : 0;
			QualitySettings.SetQualityLevel(qualityIndex); 			
			user = User.User.Load();

			// Lambda expressions.
			user.UnsavedChange(() => { features.UnlockTutorialContent(); } );
		}

	#if UNITY_EDITOR

		// Attributes, preprocessor.
		[ContextMenu("Delete `PlayerPrefs`")]
		void DeletePlayerPrefs()
		{ PlayerPrefs.DeleteAll(); }

	#endif

		// Parameters.
		void OnApplicationPause(bool isPaused)
		{
			if (Application.isEditor) return;
			if (isPaused) Save();
		}


	#region UI Events

		// Region.
		public void OnTutorialFinish(string level)
		{
			Features.UnlockAllContent();
			UI.ShowCompletionMessage("Tutorial completed!".Localized());
		}

	#endregion


	#region Solver

		// Control keywords.
		public void StartSolver()
		{
			isStarted = true;
			StartCoroutine("Solver");
		}

		IEnumerator Solver()
		{
			while (isStarted)
			{
				try
				{ Solver.Solve(); }
				catch (System.Exception)
				{ break; }
				
				if (solveAgain) continue;
				yield return new WaitForSeconds(2.0f);
			}

			yield return null;
		}

	#endregion

	// Operators, local variables, XML Documentation.
	#region Helpers

			/// <summary>
   			/// Measures point-line distance.
   			/// </summary>
   			/// <param name="point">Point to measure distance to.</param>
			/// <param name="a">A point on the line.</param>
			/// <param name="b">Another point on the line.</param>		   
			public static float PointDistanceFromLine(Vector2 point, Vector2 a, Vector2 b)
			{
				float a_ = point.x - a.x;
				float b_ = point.y - a.y;
				float c_ = b.x - a.x;
				float d_ = b.y - a.y;
				return Mathf.Abs(a_ * d_ - c_ * b_) / Mathf.Sqrt(c_ * c_ + d_ * d_);
			}

	#endregion

	}


	#region Settings

		// Enums, attribute parameters.
		 [CreateAssetMenu(menuName = "Settings", fileName = "Settings")]
		public class Settings : ScriptableObject
		{


			public bool sounds;
			public enum Difficulty { Easy, Moderate, Hard }
			public Difficulty difficulty;
		}

	#endregion


}