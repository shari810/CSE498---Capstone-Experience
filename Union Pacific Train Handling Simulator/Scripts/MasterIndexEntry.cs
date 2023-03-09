//using UnityEngine;
//using System.Collections;

//public class MasterIndexEntry
//{	
//	public enum EntryType : byte
//	{
//		TrackPoint		= 0,
//		Track			= 1,
//		TrackNode		= 2,
//		TrackMarker		= 3,
//		Consist			= 4,
//		Car				= 5,
//		Script			= 6,
//		Interaction		= 7,
//		Settings		= 8,
//		InstanceGlobals	= 9,
//		SceneryTile		= 10,
//		Scenery			= 11,
//		ModelReference	= 12,
//		ModelType		= 13,
//		Road			= 14,
//		Extrusion		= 15,
//		TrackEnd		= 16,
//		CarView			= 17,
//		Avatar			= 18,
//		CarAttachment	= 19,
//		Spline			= 20
//	}

//	public int uniqueId  = -1;
//	public MasterIndexEntry.EntryType	type;
//	public int	selectionId = -1;

//	public MasterIndexEntry(EntryType entryType)
//	{
//		TmtsMasterIndex.AddEntry(this, entryType);
//	}
	
//	public virtual void SetSelected(bool isSelected)
//	{

//	}

//	public virtual string ToDataDebugString()
//	{
//		return string.Empty;
//	}
//}