using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LBS.Components.TileMap
{
    [System.Serializable]
    public class LBSSchema : AreaTileMap<TiledArea> 
    {
        public LBSSchema() : base()
        {
            areas = new List<TiledArea>();
        }

        public LBSSchema(List<TiledArea> areas, string key) : base(areas, key) { }

        public override object Clone()
        {
            var x = areas.Select(a => a.Clone() as TiledArea).ToList();
            return new LBSSchema(x, key);
        }
    }

}