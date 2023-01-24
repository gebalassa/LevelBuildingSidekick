using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using LBS.Components.Teselation;

namespace LBS.Components.TileMap
{
    public class AreaTileMap<T,U> : TeselationModule where T : TiledArea<U> where U : LBSTile
    {
        public List<T> areas;

        public int RoomCount => areas.Count;

        public bool AddArea(T room)
        {
            if (room == null)
                return false;
            if (GetArea(room.ID) != null)
                return false;
            areas.Add(room);
            room.OnAddTile = (t) => 
            {
                RemoveTile(t);
            };
            return true;
        }

        private void RemoveTile(U t)
        {
            foreach(var r in areas)
            {
                if(r.Contains(t.Position))
                {
                    r.RemoveTile(t);
                }
            }
        }

        public T GetArea(string id)
        {
            return areas.Find(r => r.Key == id);
        }

        public T GetRoom(int index)
        {
            return areas[index];
        }

        public bool RemoveRoom(T area)
        {
            return areas.Remove(area);
        }

        private int GetRoomDistance(string r1, string r2) // O2 - manhattan
        {
            var lessDist = int.MaxValue;
            var room1 = GetArea(r1);
            var room2 = GetArea(r2);
            for (int i = 0; i < room1.TileCount; i++)
            {
                var dist = room2.GetDistance(room1.GetTile(i).Position);

                if (dist <= lessDist)
                {
                    lessDist = dist;
                }
            }
            return lessDist;
        }

        public override void Clear()
        {
            areas.Clear();
        }

        public override void Print()
        {
            throw new System.NotImplementedException();
        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }

    }
    
}

