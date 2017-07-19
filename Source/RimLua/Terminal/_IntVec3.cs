using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using OtherIntVec = Verse.IntVec3;

namespace RimLua.Terminal
{
    public struct IntVec3
    {
        public int x;
        public int y;
        public int z;
        #region Copied from Verse.IntVec3.


        public IntVec2 ToIntVec2
        {
            get
            {
                return new IntVec2(x, z);
            }
        }

        public bool IsValid
        {
            get
            {
                return y >= 0;
            }
        }

        public int LengthHorizontalSquared
        {
            get
            {
                return x * x + z * z;
            }
        }

        public float LengthHorizontal
        {
            get
            {
                return GenMath.Sqrt((x * x + z * z));
            }
        }

        public int LengthManhattan
        {
            get
            {
                return ((x < 0) ? (-x) : x) + ((z < 0) ? (-z) : z);
            }
        }

        public float AngleFlat
        {
            get
            {
                if (x == 0 && z == 0)
                {
                    return 0f;
                }
                return Quaternion.LookRotation(ToVector3()).eulerAngles.y;
            }
        }

        public static IntVec3 Zero
        {
            get
            {
                return new IntVec3(0, 0, 0);
            }
        }

        public static IntVec3 North
        {
            get
            {
                return new IntVec3(0, 0, 1);
            }
        }

        public static IntVec3 East
        {
            get
            {
                return new IntVec3(1, 0, 0);
            }
        }

        public static IntVec3 South
        {
            get
            {
                return new IntVec3(0, 0, -1);
            }
        }

        public static IntVec3 West
        {
            get
            {
                return new IntVec3(-1, 0, 0);
            }
        }

        public static IntVec3 Invalid
        {
            get
            {
                return new IntVec3(-1000, -1000, -1000);
            }
        }

        public IntVec3(int newX, int newY, int newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        public IntVec3(Vector3 v)
        {
            x = (int)v.x;
            y = 0;
            z = (int)v.z;
        }

        public IntVec3(Vector2 v)
        {
            x = (int)v.x;
            y = 0;
            z = (int)v.y;
        }

        public static IntVec3 FromString(string str)
        {
            str = str.TrimStart(new char[]
            {
                '('
            });
            str = str.TrimEnd(new char[]
            {
                ')'
            });
            string[] array = str.Split(new char[]
            {
                ','
            });
            IntVec3 result;
            try
            {
                int newX = Convert.ToInt32(array[0]);
                int newY = Convert.ToInt32(array[1]);
                int newZ = Convert.ToInt32(array[2]);
                result = new IntVec3(newX, newY, newZ);
            }
            catch (Exception arg)
            {
                Log.Warning(str + " is not a valid IntVec3 format. Exception: " + arg);
                result = IntVec3.Invalid;
            }
            return result;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public Vector3 ToVector3Shifted()
        {
            return new Vector3(x + 0.5f, y, z + 0.5f);
        }

        public Vector3 ToVector3ShiftedWithAltitude(AltitudeLayer AltLayer)
        {
            return ToVector3ShiftedWithAltitude(Altitudes.AltitudeFor(AltLayer));
        }

        public Vector3 ToVector3ShiftedWithAltitude(float AddedAltitude)
        {
            return new Vector3(x + 0.5f, y + AddedAltitude, z + 0.5f);
        }

        public bool InHorDistOf(IntVec3 otherLoc, float maxDist)
        {
            float num = (x - otherLoc.x);
            float num2 = (z - otherLoc.z);
            return num * num + num2 * num2 <= maxDist * maxDist;
        }

        public static IntVec3 FromVector3(Vector3 v)
        {
            return IntVec3.FromVector3(v, 0);
        }

        public static IntVec3 FromVector3(Vector3 v, int newY)
        {
            return new IntVec3((int)v.x, newY, (int)v.z);
        }

        public Vector2 ToUIPosition()
        {
            return ToVector3Shifted().MapToUIPosition();
        }

        public bool AdjacentToCardinal(IntVec3 other)
        {
            return IsValid && ((other.z == z && (other.x == x + 1 || other.x == x - 1)) || (other.x == x && (other.z == z + 1 || other.z == z - 1)));
        }

        public bool AdjacentToDiagonal(IntVec3 other)
        {
            return IsValid && Mathf.Abs(x - other.x) == 1 && Mathf.Abs(z - other.z) == 1;
        }

        public bool AdjacentToCardinal(Room room)
        {
            if (!IsValid)
            {
                return false;
            }
            Map map = room.Map;
            if (GenGrid.InBounds(this, map) && GridsUtility.GetRoom(this, map, RegionType.Set_All) == room)
            {
                return true;
            }
            OtherIntVec[] cardinalDirections = GenAdj.CardinalDirections;
            for (int i = 0; i < cardinalDirections.Length; i++)
            {
                IntVec3 intVec = this + (IntVec3)cardinalDirections[i];
                if (GenGrid.InBounds(this, map) && GridsUtility.GetRoom(intVec, map, RegionType.Set_All) == room)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            return obj is IntVec3 && Equals((IntVec3)obj);
        }

        public bool Equals(IntVec3 other)
        {
            return x == other.x && z == other.z && y == other.y;
        }

        public override int GetHashCode()
        {
            int seed = 0;
            seed = Gen.HashCombineInt(seed, x);
            seed = Gen.HashCombineInt(seed, y);
            return Gen.HashCombineInt(seed, z);
        }

        public ulong UniqueHashCode()
        {
            ulong num = 0uL;
            num += (ulong)(1L * (long)x);
            num += (ulong)(4096L * (long)z);
            return num + (ulong)(16777216L * (long)y);
        }

        public override string ToString()
        {
            return string.Concat(new string[]
            {
                "(",
                x.ToString(),
                ", ",
                y.ToString(),
                ", ",
                z.ToString(),
                ")"
            });
        }

        public static IntVec3 operator +(IntVec3 a, IntVec3 b)
        {
            return new IntVec3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static IntVec3 operator -(IntVec3 a, IntVec3 b)
        {
            return new IntVec3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static IntVec3 operator *(IntVec3 a, int i)
        {
            return new IntVec3(a.x * i, a.y * i, a.z * i);
        }

        public static bool operator ==(IntVec3 a, IntVec3 b)
        {
            return a.x == b.x && a.z == b.z && a.y == b.y;
        }

        public static bool operator !=(IntVec3 a, IntVec3 b)
        {
            return a.x != b.x || a.z != b.z || a.y != b.y;
        }
        #endregion
        public static implicit operator OtherIntVec(IntVec3 c)
        {
            return new OtherIntVec(c.x, c.y, c.z);
        }
        public static implicit operator IntVec3(OtherIntVec c)
        {
            return new IntVec3(c.x, c.y, c.z);
        }
    }
}
