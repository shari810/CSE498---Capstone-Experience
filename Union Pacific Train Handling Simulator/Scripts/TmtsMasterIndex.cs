//using UnityEngine;
//using System;
//using System.Collections.Generic;

//public class TmtsMasterIndex : ScriptableObject
//{
//	private static List<MasterIndexEntry>	entries;
//	private static List<int>				orphans;
//	private static List<HashSet<int>>		idsOfType;
	
//	static TmtsMasterIndex()
//	{
//		entries = new List<MasterIndexEntry>();
//		orphans = new List<int>();
//		idsOfType = new List<HashSet<int>>(Enum.GetValues(typeof(MasterIndexEntry.EntryType)).Length);
//		for(int i = 0; i < idsOfType.Capacity; i++)
//		{
//			idsOfType.Add( new HashSet<int>() );
//		}
//	}
	
//	public static void AddEntry(MasterIndexEntry entry, MasterIndexEntry.EntryType type)
//	{	
//		int id;
		
//		if (orphans.Count > 0)
//		{
//			id = orphans[orphans.Count-1];
//			orphans.RemoveAt(orphans.Count-1);
//			entries[id] = entry;
//		}
//		else
//		{
//			id = entries.Count;
//			entries.Add(entry);
//		}
		
//		entry.type = type;
//		entry.uniqueId = id;
		
//		idsOfType[(int)type].Add(id);
//	}
	
//	public static MasterIndexEntry GetEntry(int id)
//	{
//		if (id < 0 || id >= entries.Count)
//		{
//			return null;
//		}
		
//		return entries[id];
//	}
	
//	public static void RemoveEntry(MasterIndexEntry entry)
//	{	
//		idsOfType [(int)entry.type].Remove(entry.uniqueId);
//		int		id = entry.uniqueId;
//		entries[id] = null;
//		orphans.Add(id);
//	}

//	public static void RemoveAllTrack()
//	{
//		for (int i = 0; i < entries.Count; i++)
//		{
//			if (entries[i] != null)
//			{
//				if (entries[i].type <= MasterIndexEntry.EntryType.TrackMarker)
//				{
//					RemoveEntry(entries[i]);
//				}
//			}
//		}
//	}

//	public static bool ReplaceEntry(MasterIndexEntry oldEntry, MasterIndexEntry newEntry)
//	{
//		if (oldEntry.uniqueId < 0 || oldEntry.uniqueId >= entries.Count)
//		{
//			return false;
//		}
//		if (oldEntry != entries[oldEntry.uniqueId])
//		{
//			return false;
//		}
		
//		if (newEntry.uniqueId >= 0 && newEntry.uniqueId < entries.Count)
//		{
//			if (newEntry == entries[newEntry.uniqueId])
//			{
//				entries[newEntry.uniqueId] = null;
//				orphans.Add(newEntry.uniqueId);
//			}
//		}

//		entries[oldEntry.uniqueId] = newEntry;
//		newEntry.uniqueId = oldEntry.uniqueId;
		
//		idsOfType[(int)oldEntry.type].Remove(oldEntry.uniqueId);
//		idsOfType[(int)newEntry.type].Add(newEntry.uniqueId);
		
//		return true;
//	}

//	public static HashSet<int> GetIdsOfEntriesWithType(MasterIndexEntry.EntryType type)
//	{
//		return idsOfType[(int)type];
//	}	
	
//	public static void Reset()
//	{
//		if (entries != null)
//			entries.Clear ();
		
//		if (entries != null)
//			orphans.Clear ();
		
//		if (idsOfType != null)
//		{
//			foreach(HashSet<int> hashSet in idsOfType)
//			{
//				hashSet.Clear();	
//			}
//		}
//	}
	
//	public static int Count
//	{
//		get{ return entries.Count; }	
//	}
//}