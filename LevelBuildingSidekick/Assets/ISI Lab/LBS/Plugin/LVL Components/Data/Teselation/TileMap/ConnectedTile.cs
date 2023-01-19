using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LBS.Components.TileMap
{
    public class ConnectedTile : LBSTile
    {
        string[] connections;

        public ConnectedTile() : base ()
        {
            connections = new string[Sides];
        }

        public ConnectedTile(Vector2 position, string id, int sides = 4) : base(position, id, sides)
        {
            connections = new string[Sides];
        }

        public bool SetConnection(string connection, int index)
        {
            if(!connections.ContainsIndex(index))
            {
                return false;
            }

            if(connections[index] == connection)
            {
                return false;
            }

            connections[index] = connection;
            return true;
        }

        public string GetConnection(int index)
        {
            if(connections.ContainsIndex(index))
            {
                return connections[index];
            }
            return null;
        }

        public bool RemoveConnection(string connection)
        {
            throw new NotImplementedException();
        }
    }
}

