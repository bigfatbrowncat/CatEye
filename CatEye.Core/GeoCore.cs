using System;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;

namespace CatEye.Core
{
	public class SelfCrossingException : Exception
	{
		public SelfCrossingException(string message) : base(message) { }
	}
	public class ZeroVectorsAngleException : Exception
	{
		public ZeroVectorsAngleException(string message) : base(message) { }
	}
	public class PolygonNotConvexException : Exception
	{
		public PolygonNotConvexException(string message) : base(message) { }
	}
	
	public class Point
	{
		private NumberFormatInfo nfi = NumberFormatInfo.InvariantInfo;

		private double _X, _Y;
		public double X { get { return _X; } }
		public double Y { get { return _Y; } }
		public Point(double x, double y)
		{
			_X = x; _Y = y;
		}
		
		public static implicit operator Vector(Point p)
		{
			return new Vector(p._X, p._Y);
		}
		
		public static Point Rotate(Point src, double angle, Point center)
		{
			double x = src.X - center.X, y = src.Y - center.Y;
			
			return new Point(
				x * Math.Cos(angle) - y * Math.Sin(angle) + center.X,
				y * Math.Cos(angle) + x * Math.Sin(angle) + center.Y
			);
		} 
		public System.Xml.XmlNode SerializeToXML (System.Xml.XmlDocument xdoc)
		{
			XmlNode xn = xdoc.CreateElement("Point");
			xn.Attributes.Append(xdoc.CreateAttribute("X")).Value = _X.ToString(nfi);
			xn.Attributes.Append(xdoc.CreateAttribute("Y")).Value = _Y.ToString(nfi);
			return xn;
		}
		
		public void DeserializeFromXML (XmlNode node)
		{
			if (node.Name != "Point")
				throw new IncorrectNodeException("Node should have name \"Point\"");
			
			double res = 0;
			if (node.Attributes["X"] != null)
			{
				if (double.TryParse(node.Attributes["X"].Value, NumberStyles.Float, nfi, out res))
				{
					_X = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse R value");
			}
			if (node.Attributes["Y"] != null)
			{
				if (double.TryParse(node.Attributes["Y"].Value, NumberStyles.Float, nfi, out res))
				{
					_Y = res;
				}
				else
					throw new IncorrectNodeValueException("Can't parse G value");
			}
		}
		
		public static Point Move(Point p, Vector v)
		{
			return new Point(p._X + v.X, p._Y + v.Y);
		}
		
	}
	
	public class Segment
	{
		private Point _A, _B;
		public Point A { get { return _A; } }
		public Point B { get { return _B; } }
		
		private Dictionary<Segment, Point> _Crossings = new Dictionary<Segment, Point>();
		
		public Segment HasCrossing(Point crossing)
		{
			foreach (Segment s in _Crossings.Keys)
			{
				if (_Crossings[s] == crossing) return s;
			}
			return null;
		}
		
		public Segment(Point a, Point b)
		{
			_A = a; _B = b;
		}
		
		public static Point GetCrossing(Segment s1, Segment s2)
		{
			// Algorithmic equality
			if (s1 == s2) 
				throw new SelfCrossingException("Trying to find selfcrossing");
			
			// TODO: Add rough estimation here (i.e. rectangle estimation)

			if (s1._Crossings.ContainsKey(s2)) 
				return s1._Crossings[s2];
			// TODO: Check integrity
			
			// Parallelism and zero-length
			Vector AB1 = new Vector(s1._B.X - s1._A.X, s1._B.Y - s1._A.Y);
			Vector AB2 = new Vector(s2._B.X - s2._A.X, s2._B.Y - s2._A.Y);
			double D = Vector.CrossProduct(AB2, AB1);
			if (Math.Abs(D) == 0) return null;
			
			if (s1._A == s2._A || s1._A == s2._B) return s1._A;
			if (s1._B == s2._A || s1._B == s2._B) return s1._B;
		
			// Calculating crossing between lines
			double m = Vector.CrossProduct((Vector)s1._A, AB1);
			double n = Vector.CrossProduct((Vector)s2._A, AB2);
			
			double x = (m * AB2.X - n * AB1.X) / D;
			double y = (m * AB2.Y - n * AB1.Y) / D;

			// Checking if an endpoint of one segment is contained by other
			if (s1._A.X == x && s1._A.Y == y) return s1._A; 
			if (s1._B.X == x && s1._B.Y == y) return s1._B; 
			if (s2._A.X == x && s2._A.Y == y) return s2._A; 
			if (s2._B.X == x && s2._B.Y == y) return s2._B; 
			
			// Checking if the crossing point is in both segments
			Vector AC1 = new Vector(x - s1._A.X, y - s1._A.Y);
			Vector BC1 = new Vector(x - s1._B.X, y - s1._B.Y);
			if ((AB1 * AC1) * (-AB1 * BC1) < 0) return null;
			
			Vector AC2 = new Vector(x - s2._A.X, y - s2._A.Y);
			Vector BC2 = new Vector(x - s2._B.X, y - s2._B.Y);
			if ((AB2 * AC2) * (-AB2 * BC2) < 0) return null;
			
			// Adding the point to crossing dictionaries
			Point crs = new Point(x, y);
			s1._Crossings.Add(s2, crs);
			s2._Crossings.Add(s1, crs);
			
			return crs;
		}
	}
	
	public class ConvexPolygon
	{
		private Point[] mVertex;
		private Segment[] mSides;
		private double mXMin, mXMax, mYMin, mYMax;
		
		public double XMin { get { return mXMin; } }
		public double XMax { get { return mXMax; } }
		public double YMin { get { return mYMin; } }
		public double YMax { get { return mYMax; } }
		
		public Point[] Vertex
		{
			get { return mVertex; } 
		}
		public Segment[] Sides
		{
			get { return mSides; } 
		}
		
		
		public ConvexPolygon(Point[] points)
		{
			if (points.Length < 3)
			{
				throw new ArgumentException("Too few points (" + points.Length + "). There should be at least 3 points to make a polygon");
			}
			// Checking convexity
			double sign_value = Vector.CrossProduct(
				(Vector)points[0] - (Vector)points[points.Length - 1],
				(Vector)points[1] - (Vector)points[points.Length - 1]);
			
			double sign_value2 = Vector.CrossProduct(
				(Vector)points[points.Length - 1] - (Vector)points[points.Length - 2],
				(Vector)points[0] - (Vector)points[points.Length - 2]);
			if (Math.Sign(sign_value) * Math.Sign(sign_value2) < 0) 
				throw new PolygonNotConvexException("Polygon isn't convex");
			
			for (int i = 0; i < points.Length - 2; i++)
			{
				double sign_value_i = Vector.CrossProduct(
					(Vector)points[i + 1] - (Vector)points[i],
					(Vector)points[i + 2] - (Vector)points[i]);
				if (Math.Sign(sign_value) * Math.Sign(sign_value_i) < 0) 
					throw new PolygonNotConvexException("Polygon isn't convex");
			}
			
			// Sorting points clockwise
			if (sign_value > 0)
				mVertex = points;
			else
			{
				mVertex = new Point[points.Length];
				for (int i = 0; i < points.Length; i++)
				{
					mVertex[i] = points[points.Length - 1 - i];
				}
			}
			
			// Constructing sides
			mSides = new Segment[mVertex.Length];
			mSides[mVertex.Length - 1] = new Segment(mVertex[mVertex.Length - 1], mVertex[0]);
			for (int i = 0; i < mVertex.Length - 1; i++)
				mSides[i] = new Segment(mVertex[i], mVertex[i + 1]);
			
			// Calculating bounding box
			mXMin = mVertex[0].X; mXMax = mVertex[0].X;
			mYMin = mVertex[0].Y; mYMax = mVertex[0].Y;
			
			for (int i = 1; i < mVertex.Length; i++)
			{
				if (mXMin > mVertex[i].X) mXMin = mVertex[i].X;
				if (mYMin > mVertex[i].Y) mYMin = mVertex[i].Y;

				if (mXMax < mVertex[i].X) mXMax = mVertex[i].X;
				if (mYMax < mVertex[i].Y) mYMax = mVertex[i].Y;
			}
		}
		
		public int IndexOfSideWithAEnd(Point A)
		{
			for (int i = 0; i < mSides.Length; i++)
				if (A == mSides[i].A) return i;
			return -1;
		}
		public int IndexOfSideWithBEnd(Point B)
		{
			for (int i = 0; i < mSides.Length; i++)
				if (B == mSides[i].B) return i;
			return -1;
		}
		public int IndexOfSide(Segment side)
		{
			for (int i = 0; i < mSides.Length; i++)
				if (mSides[i] == side) return i;
			return -1;
		}
		
		public bool HasCrossing(Point crossing, out Segment mySide, out Segment other)
		{
			mySide = null; other = null;
			for (int i = 0; i < mSides.Length; i++)
			{
				Segment res = mSides[i].HasCrossing(crossing);
				if (res != null)
				{
					mySide = mSides[i];
					other = res;
					return true;
				}
			}
			return false;
		}
		
		public double GetArea()
		{
			double res = 0;
			for (int i = 1; i < mVertex.Length - 1; i++)
			{
				Vector a = new Vector(mVertex[i].X - mVertex[0].X,
									  mVertex[i].Y - mVertex[0].Y);
				Vector b = new Vector(mVertex[i + 1].X - mVertex[0].X,
									  mVertex[i + 1].Y - mVertex[0].Y);
				res += Vector.CrossProduct(a, b);
			}
			res /= 2;
			return res;
		}
		
		public static bool BoundingBoxesAreCrossed(ConvexPolygon p1, ConvexPolygon p2)
		{
			if (p1.mXMin > p2.mXMax) return false;
			if (p2.mXMin > p1.mXMax) return false;
			
			if (p1.mYMin > p2.mYMax) return false;
			if (p2.mYMin > p1.mYMax) return false;
			return true;
		}
		
		public bool Contains(Point p)
		{
			Vector a0 = new Vector(
				mVertex[0].X - mVertex[mVertex.Length - 1].X,
				mVertex[0].Y - mVertex[mVertex.Length - 1].Y);
			Vector b0 = new Vector(
				p.X - mVertex[mVertex.Length - 1].X,
				p.Y - mVertex[mVertex.Length - 1].Y);
			if (Vector.CrossProduct(a0, b0) <= 0) return false;
			
			for (int i = 1; i < mVertex.Length; i++)
			{
				Vector a = new Vector(
					mVertex[i].X - mVertex[i - 1].X,
					mVertex[i].Y - mVertex[i - 1].Y);
				Vector b = new Vector(
					p.X - mVertex[i - 1].X,
					p.Y - mVertex[i - 1].Y);
				if (Vector.CrossProduct(a, b) <= 0) return false;
			}
			return true;
		}
		
		public Point[] Cross(Segment s, out Segment[] sides)
		{
			int k = 0;
			Point[] ps = new Point[2];
			Segment[] seg = new Segment[2];
			for (int i = 0; i < mSides.Length; i++)
			{
				Point cr = Segment.GetCrossing(s, mSides[i]);
				if (cr != null) 
				{ 
					ps[k] = cr; 
					seg[k] = mSides[i];
					k++; 
				}
			}
			if (k == 0) 
			{
				sides = new Segment[] {};
				return new Point[] {};
			}
			else if (k == 1) 
			{
				sides = new Segment[] { seg[0] };
				return new Point[] { ps[0] };
			}
			else 
			{
				if (new Vector(s.A.X - ps[0].X, s.A.Y - ps[0].Y).Length <
					new Vector(s.A.X - ps[1].X, s.A.Y - ps[1].Y).Length)
				{
					sides = seg;
					return ps;
				}
				else
				{
					sides = new Segment[] { seg[1], seg[0] };
					return new Point[] { ps[1], ps[0] };
				}
			}			
		}
		
		
		public static ConvexPolygon Cross(ConvexPolygon p1, ConvexPolygon p2)
		{
			// Checking bounding boxes
			if (!BoundingBoxesAreCrossed(p1, p2))
			{
				return null;
			}
			
			Point cur_point = null;
			// Searching for first outer point
			for (int i = 0; i < p2.mVertex.Length; i++)
			{
				if (!p1.Contains(p2.mVertex[i])) 
				{
					cur_point = p2.mVertex[i];
					break;
				}
			}
			if (cur_point == null) 
			{
				// This case means that p2 is fully contained inside p1,
				// so p2 is the crossing of p1 and p2 itself.
				return p2;
			}
			
			// Ok. The point is found. Now we start to trace path from it.
			List<Point> trace = new List<Point>();
			ConvexPolygon cur_p = p2;
			ConvexPolygon other_p = p1;
			int start_point_index = cur_p.IndexOfSideWithAEnd(cur_point);
			int cur_point_index = start_point_index;
			bool start = true;
			do
			{
				Segment[] segs;
				Point[] crs = other_p.Cross(cur_p.mSides[cur_point_index], out segs);
				// Ok. Now let's cross the possible cases out:
				
				if (other_p.Contains(cur_p.mSides[cur_point_index].A))
				{
					// 1. No crossings
					if (crs.Length == 0)
					{
						// Add B end to the trace. (A is already there)
						trace.Add(cur_p.mSides[cur_point_index].B);
					}
					// 2. One crossing.
					if (crs.Length == 1)
					{
						// The other (B) end is outside. 
						// At first we should add the crossing to trace.
						trace.Add(crs[0]);
						
						// Now we should calculate the new current tracing side
						// Searching for the side of other containing current crossing
						
						Segment other_side = segs[0];
						//if (!other_p.HasCrossing(crs[0], out other_side, out cur_side))
						//	throw new Exception("Something strange occured!");
						
						// Adding it's B point to trace
						//trace.Add(other_side.B);
						
						// Welcome, new cur_point_index (we subtract 1 which would be added automatically later)
						cur_point_index = other_p.IndexOfSide(other_side) - 1; //.IndexOfSideWithAEnd(other_side.B) - 1;
						
						// We don't care about start_point_index anymore cause at least one
						// point is already in the trace.
						
						// After that we should swap tracing polygons
						ConvexPolygon tmp = cur_p;
						cur_p = other_p;
						other_p = tmp;
						
						// Now we continue tracing with other polygon as the current.
						
					}
					// 3. Two crossings
					if (crs.Length == 2)
					{
						// Aaaa!!! It couldn't be! The polygons have to be convex.
						throw new PolygonNotConvexException("Not convex polygon found during tracing");
					}
				}
				else
				{
					if (crs.Length == 0)
					{
						// Do nothing. The whole side is outside the other_p
					}
					if (crs.Length == 1)
					{
						if (start)
						{
							trace.Add(crs[0]);
							start = false;
						}
						// The other (B) end is inside. 
						// We should add the crossing and the B end to trace.
						trace.Add(cur_p.mSides[cur_point_index].B);
					}
					if (crs.Length == 2)
					{
						if (start)
						{
							trace.Add(crs[0]);
							start = false;
						}
						// Both ends are outside. 
						// At first we should add first crossing to trace.
						trace.Add(crs[1]);
					
						// Now we should calculate the new current tracing side
						// Searching for the side of other containing current second crossing 
						// (the crossing where we get out)
						Segment other_side = segs[1];
						//if (!other_p.HasCrossing(crs[1], out other_side, out cur_side))
						//	throw new Exception("Something strange occured!");
						
						// Welcome, new cur_point_index (we subtract 1 which would be added automatically later)
						cur_point_index = other_p.IndexOfSide(other_side) - 1;
						
						// We don't care about start_point_index anymore cause at least one
						// point is already in the trace.
						
						// After that we should swap tracing polygons
						ConvexPolygon tmp = cur_p;
						cur_p = other_p;
						other_p = tmp;
						
						// Now we continue tracing with other polygon as the current.
					}
				}
				
				cur_point_index++;
				if (cur_point_index > cur_p.mSides.Length - 1) cur_point_index = 0;
			}
			while (!((trace.Count == 0 && cur_point_index == start_point_index) || 
				   (trace.Count > 1 && trace[0] == trace[trace.Count - 1])));
			
			if (trace.Count > 0)
			{
				trace.RemoveAt(trace.Count - 1);
				
				return new ConvexPolygon(trace.ToArray());
			}
			else
				return null;
			
		}
	}
	
    public class Vector
    {
        private double m_x, m_y;
        public double X
        {
            get { return m_x; }
            set { m_x = value; }
        }

        public double Y
        {
            get { return m_y; }
            set { m_y = value; }
        }

        public Vector(double x, double y)
        {
            m_x = x;
            m_y = y;

        }

        /// <summary>
        /// "Длина" -- модуль вектора
        /// </summary>
        public double Length
        {
            get
            {
                return Math.Sqrt(m_x * m_x + m_y * m_y);
            }
        }

        /// <summary>
        /// Нуль-вектор
        /// </summary>
        public static Vector Zero
        {
            get
            {
                return new Vector(0, 0);
            }
        }

        /// <summary>
        /// Умножение вектора на число
        /// </summary>
        public static Vector operator *(Vector a, double k)
        {
            Vector res = a;
            res.m_x *= k;
            res.m_y *= k;
            return res;
        }

        /// <summary>
        /// Деление вектора на число
        /// </summary>
        public static Vector operator /(Vector a, double k)
        {
            return a * (1.0 / k);
        }

        /// <summary>
        /// Умножение числа на вектор
        /// </summary>
        public static Vector operator *(double k, Vector a)
        {
            return a * k;
        }

        /// <summary>
        /// Скалярное произведение
        /// </summary>
        public static double operator *(Vector a, Vector b)
        {
            return a.m_x * b.m_x + a.m_y * b.m_y;
        }

        /// <summary>
        /// Сложение двух векторов
        /// </summary>
        public static Vector operator +(Vector a, Vector b)
        {
            Vector res = a;
            res.m_x += b.m_x;
            res.m_y += b.m_y;
            return res;

        }

        /// <summary>
        /// Вычитание двух векторов
        /// </summary>
        public static Vector operator -(Vector a, Vector b)
        {
            Vector res = a;
            res.m_x -= b.m_x;
            res.m_y -= b.m_y;
            return res;

        }

        /// <summary>
        /// Унарный минус
        /// </summary>
        public static Vector operator -(Vector a)
        {
            return a * (-1.0);
        }

        /// <summary>
        /// Векторное произведение
        /// </summary>
        public static double CrossProduct(Vector a, Vector b)
        {
            return a.m_x * b.m_y - a.m_y * b.m_x;
        }

        /// <summary>
        /// Угол между векторами
        /// </summary>
        public static double Angle(Vector a, Vector b, double epsilon)
        {
            double q = Math.Asin(CrossProduct(a, b) / (a.Length * b.Length));
            if (a.Length < epsilon || b.Length < epsilon)
                throw new ZeroVectorsAngleException("Trying to find angle between zero-length vectors (epsilon = " + epsilon + ")");
            else
                return q;
        }

		public Vector(Point begin, Point end)
		{
			this.m_x = end.X - begin.X;
			this.m_y = end.Y - begin.Y;
		}
		
        public static implicit operator Point(Vector a)
        {
            return new Point(a.m_x, a.m_y);
        }

    }

}

