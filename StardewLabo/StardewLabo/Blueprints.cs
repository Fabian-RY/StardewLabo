using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewLabo
{

    public class BluePrints
    {

        private int daysToBuild = 0;
        private int moneyCost = 10000;

        public BluePrint LaboBlueprint(IModHelper helper)
        {
            BluePrint bp = new BluePrint("Shed")
            {
                name = "Laboratory",
                displayName = "Laboratorio",
                description = "Permite almacenar máquinas biotecnológicas",
                daysToConstruct = daysToBuild,//4
                moneyRequired = moneyCost, //40000
                tilesWidth = 7,
                tilesHeight = 3,
                humanDoor = new Point(3, 2)
            };
            bp.itemsRequired.Clear();
            bp.itemsRequired.Add(709, 100);//200
            bp.itemsRequired.Add(330, 100);//100
            bp.itemsRequired.Add(390, 100);//100
            helper.Reflection.GetField<object>(bp, "textureName").SetValue("buildings/Laboratory");
            helper.Reflection.GetField<object>(bp, "texture").SetValue(Game1.content.Load<Texture2D>(bp.textureName));
            //this.Helper.Reflection.GetField<object>(LaboBluePrint, "mapToWarpTo").SetValue(this.);
            return bp;
        }
    }
}
