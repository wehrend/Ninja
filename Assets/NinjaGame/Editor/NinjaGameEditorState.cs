using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using Assets.NinjaGame.Scripts;

namespace Assets.NinjaGame.Editor
{
	public class NinjaGameEditorState : ScriptableObject
	{
		public List<PrefabContainer> selectablePrefabs;
		
	
		void OnEnable() {
			if( selectablePrefabs== null || !selectablePrefabs.Any())
				selectablePrefabs = LookUpPrefabs();
		}
		
		private List<PrefabContainer> LookUpPrefabs()
		{
			var result = new List<PrefabContainer>();
			//load our resources 
			var prefabs = Resources.FindObjectsOfTypeAll(typeof(MovingRigidbodyPhysics));
			
			foreach (var item in prefabs )
			{
				var aLoadedPrefabView = new PrefabContainer();
            
				aLoadedPrefabView.referenceToPrefab = (GameObject) item;
				aLoadedPrefabView.isSelected = false;
				
				result.Add(aLoadedPrefabView);
				
			}
			
			return result;
		}
	}
		

	
	
[Serializable]

public class PrefabContainer
{
	public GameObject referenceToPrefab;
	public bool isSelected;
}
	
}