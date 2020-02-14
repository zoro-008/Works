using Emgu.CV;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace VDll
{
    class Map
    {
        Texture2D img   ;
        Sight     sight ; //카메라 연결고리
        HUD       hud   ; //텍스트 연결고리

        int  numElements = 0    ;
        bool reverse     = false; //높이 반전
        
		//public  static Map Instance { get { return instance == null ? (instance = new Map()) : instance; } }
        //private static Map instance = null;
		
        public GameWindow window;
        
		public Map(bool reverse = false)
		{
            this.window = new OpenTK.GameWindow(800,600,new OpenTK.Graphics.GraphicsMode(32,8,0,0));
            img = new Texture2D();
            
            //window.Run(1.0/30.0);
            //Event Overload
            window.Load        += Window_Load;
            window.UpdateFrame += Window_UpdateFrame;
            window.RenderFrame += Window_RenderFrame;
            window.Resize      += Window_Resize ;
            //window.Closing     += Window_Closing;
            //Singleton
            sight = Sight.Instance;
            hud   = HUD.Instance;

            this.reverse = reverse;
            //LoadBmp
            //LoadBmp(@"Images\Map.bmp");
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            e.Cancel = true;
            window.Visible = false;
        }

        public bool LoadMat(Mat mat)
        {
            img.LoadMat(mat);

            return img.Load;
        }

        public bool LoadBmp(Bitmap bitmap)
        {
            img.LoadBmp(bitmap);

            return img.Load;
        }

        public bool LoadBmp(string sPath)
        {
            if (!File.Exists(sPath)) return false;
            img = ContentPipe.LoadTexture(sPath);
            return img.Load ;
        }

        private void InitLight()
        {
            GL.FrontFace(FrontFaceDirection.Ccw); // 반시계방향이 앞면이다.
            GL.Enable(EnableCap.CullFace);       // 뒷면에 대해서는 계산하지 말라
            GL.Enable(EnableCap.Lighting);       // 빛을 사용한다.
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.ColorMaterial);
            
            GL.Light(LightName.Light0,LightParameter.Ambient ,new float[] { 1, 1, 1, 1 });
            GL.Light(LightName.Light0,LightParameter.Diffuse ,new float[] { 1, 1, 1, 1 });
            GL.Light(LightName.Light0,LightParameter.Position,new float[] { 1, 1, 1, 1 });
            GL.Light(LightName.Light0,LightParameter.Specular,new float[] { 1, 1, 1, 1 });
            
            GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
            GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, new float[] { 1, 1, 1, 1 });
            GL.Material(MaterialFace.Front, MaterialParameter.Ambient, new float[] { 0.5f, 0.5f, 0.5f, 1 });
            
        }

        #region Event
        private void Window_Load(object sender, EventArgs e)
        {
            //GL.Enable(EnableCap.Blend);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha,BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.DepthTest);

            DrawHeightMap();

            InitLight(); //광원

            hud.Load();
        }
        
        private void Window_Resize(object sender, EventArgs e)
        {
            GL.Viewport(window.ClientRectangle.X, window.ClientRectangle.Y, window.ClientRectangle.Width, window.ClientRectangle.Height);
			Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4f, window.Width / window.Height, 1.0f, 60000.0f);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref projection);

            hud.Resize(window.Width, window.Height);
        }


        private void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            sight.Update(window.Focused);
            GL.MatrixMode(MatrixMode.Modelview);
            sight.LoadMatrix();

            hud.Update();
        }


        private void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            GL.DrawElements(PrimitiveType.Triangles,numElements,DrawElementsType.UnsignedInt,IntPtr.Zero);

            hud.Render();

            window.SwapBuffers();
        }

        #endregion

        private bool DrawHeightMap()
        {
            if(img == null) return false;
            //List<Vector3> verticesList = new List<Vector3>();
            //List<uint>    indicesList  = new List<uint>();
            //List<Color>   colorList    = new List<Color>();
            
            uint indicesCnt =0 ;

            int iLeft    = 0   ; 
            int iTop     = 0   ;
            int iRight   = img.Width;
            int iBottom  = img.Height;
            int c        = img.mat.Cols        ;
            int e        = img.mat.ElementSize ;

            Vector3[] verticesList = new Vector3[(iRight-(1+iLeft))*(iBottom-(1+iTop))*4];
            uint   [] indicesList  = new uint   [(iRight-(1+iLeft))*(iBottom-(1+iTop))*6];
            Vector4[] colorList    = new Vector4[(iRight-(1+iLeft))*(iBottom-(1+iTop))*4];

            int vCnt = 0;
            int iCnt = 0;
            int cCnt = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int y=iTop+1; y<iBottom; y++) {
                for (int x=iLeft+1; x<iRight; x++) {
                    Color c1 = img.GetPixelMat(x  , y  ,c,e) ; 
                    Color c2 = img.GetPixelMat(x-1, y  ,c,e) ;
                    Color c3 = img.GetPixelMat(x-1, y-1,c,e) ;
                    Color c4 = img.GetPixelMat(x  , y-1,c,e) ;

                    int iPx   ;
                    int iPxL  ;
                    int iPxLT ;
                    int iPxT  ;
                    if(reverse)
                    {
                        iPx   = 255 - ((int)(c1.R + c1.G + c1.B) / 3) ;
                        iPxL  = 255 - ((int)(c2.R + c2.G + c2.B) / 3) ;
                        iPxLT = 255 - ((int)(c3.R + c3.G + c3.B) / 3) ;
                        iPxT  = 255 - ((int)(c4.R + c4.G + c4.B) / 3) ;
                    }
                    else {
                        iPx   = ((int)(c1.R + c1.G + c1.B) / 3) ;
                        iPxL  = ((int)(c2.R + c2.G + c2.B) / 3) ;
                        iPxLT = ((int)(c3.R + c3.G + c3.B) / 3) ;
                        iPxT  = ((int)(c4.R + c4.G + c4.B) / 3) ;
                    }

                    //현재 픽셀.
                    int iWidth  = 0;//iRight -iLeft ;
                    int iHeight = 0;//iBottom-iTop  ;
                    float multiple = 1f;
                    float gfX = ((x-iLeft-(iWidth )/2))*multiple;
                    float gfZ = ((y-iTop -(iHeight)/2))*multiple;
                    float gfY = iPx *multiple ;
        
                    //왼쪽 픽셀
                    float gfXL = ((x-iLeft-1-(iWidth )/2))*multiple;
                    float gfZL = ((y-iTop   -(iHeight)/2))*multiple;
                    float gfYL = iPxL *multiple ;
        
                    //왼쪽 위 픽셀.
                    float gfXLT = ((x-iLeft-1-(iWidth )/2))*multiple;
                    float gfZLT = ((y-iTop -1-(iHeight)/2))*multiple;
                    float gfYLT = iPxLT *multiple ;
        
                    //위 픽셀.
                    float gfXT = ((x-iLeft  -(iWidth )/2))*multiple;
                    float gfZT = ((y-iTop -1-(iHeight)/2))*multiple;
                    float gfYT = iPxT *multiple ;

                    verticesList[vCnt] = new Vector3(gfXLT, gfYLT, gfZLT); vCnt++;
                    verticesList[vCnt] = new Vector3(gfXL , gfYL , gfZL ); vCnt++;
                    verticesList[vCnt] = new Vector3(gfX  , gfY  , gfZ  ); vCnt++;
                    verticesList[vCnt] = new Vector3(gfXT , gfYT , gfZT ); vCnt++;

                    indicesList[iCnt] = indicesCnt  ; iCnt++;
                    indicesList[iCnt] = indicesCnt+1; iCnt++;
                    indicesList[iCnt] = indicesCnt+2; iCnt++;
                    indicesList[iCnt] = indicesCnt  ; iCnt++;
                    indicesList[iCnt] = indicesCnt+2; iCnt++;
                    indicesList[iCnt] = indicesCnt+3; iCnt++;
                    indicesCnt+=4;

                    colorList[cCnt] = new Vector4(c3.R/255f,c3.G/255f,c3.B/255f,c3.A/255f); cCnt++;
                    colorList[cCnt] = new Vector4(c2.R/255f,c2.G/255f,c2.B/255f,c2.A/255f); cCnt++;
                    colorList[cCnt] = new Vector4(c1.R/255f,c1.G/255f,c1.B/255f,c1.A/255f); cCnt++;
                    colorList[cCnt] = new Vector4(c4.R/255f,c4.G/255f,c4.B/255f,c4.A/255f); cCnt++;

                }
            }
            Debug.WriteLine(sw.ElapsedMilliseconds);
            //Add
            int VBO = SetVerticies(verticesList);
            int COL = SetColors   (colorList   );
            int IBO = SetIndices  (indicesList );

            Debug.WriteLine(sw.ElapsedMilliseconds);
            //VBO
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
            GL.VertexPointer(3,VertexPointerType.Float,Vector3.SizeInBytes,IntPtr.Zero);

            //COL
            GL.EnableClientState(ArrayCap.ColorArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer,COL);
            //GL.ColorPointer(4,ColorPointerType.UnsignedByte,sizeof(int),IntPtr.Zero);//Vector2.SizeInBytes*2);
            GL.ColorPointer(4,ColorPointerType.Float,Vector4.SizeInBytes,IntPtr.Zero);//Vector2.SizeInBytes*2);

            Debug.WriteLine(sw.ElapsedMilliseconds);
            //GL.EnableClientState(ArrayCap.TextureCoordArray);
            //GL.EnableClientState(ArrayCap.IndexArray);
            //GL.BindBuffer(BufferTarget.ArrayBuffer,IND);
            //GL.TexCoordPointer(2,TexCoordPointerType.Float,Vector2.SizeInBytes,IntPtr.Zero);//Vector2.SizeInBytes);

            //GL.ColorPointer(3,ColorPointerType.Float,Vector3.SizeInBytes,IntPtr.Zero);//Vector2.SizeInBytes*2);
            //GL.ColorPointer(1, ColorPointerType.UnsignedByte, sizeof(int), IntPtr.Zero);
            
            //GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer,IBO);
            //GL.DrawElements(PrimitiveType.Triangles,indices.Length,DrawElementsType.UnsignedInt,0);
            return true;
        }

		public int SetVerticies(Vector3[] verticesList, int attrib = -1) {
			int arrayId;
			GL.GenVertexArrays(1, out arrayId);
			GL.BindVertexArray(arrayId);

			//Vector3[] vertices = new Vector3[verticesList.Count];
			//for (int x = 0; x < verticesList.Count; x++) {
			//	vertices[x] = verticesList[x];
			//}
            int VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
            GL.BufferData(BufferTarget.ArrayBuffer,(IntPtr)(Vector3.SizeInBytes *verticesList.Length),
                verticesList,BufferUsageHint.StaticDraw);

			if (attrib > -1) {
				GL.EnableVertexAttribArray(attrib);
				GL.VertexAttribPointer(attrib, verticesList.Length * Vector3.SizeInBytes, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
			}
            return VBO;
		}

		public int SetColors(Vector4[] colorList, int attrib = -1) {
			//Vector4[] color = new Vector4[colorList.Count];
			//for (int x = 0; x < colorList.Count; x++) {
			//	color[x] = new Vector4(colorList[x].R/255,colorList[x].G/255f,colorList[x].B/255f,colorList[x].A/255f);
			//}
            int COL = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer,COL);
            GL.BufferData(BufferTarget.ArrayBuffer,(IntPtr)(colorList.Length*Vector4.SizeInBytes),
                colorList,BufferUsageHint.StaticDraw);

			if (attrib > -1) {
				GL.EnableVertexAttribArray(attrib);
				GL.VertexAttribPointer(attrib, colorList.Length*Vector4.SizeInBytes, VertexAttribPointerType.Byte, false, sizeof(int), 0);
			}
            return COL;
		}

		public int SetIndices(uint[] indicesList) {
			//uint[] indices = new uint[indicesList.Count];
			//for (int x = 0; x < indicesList.Count; x++) {
			//	indices[x] = indicesList[x];
			//}
            int IBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint)*indicesList.Length),
                indicesList, BufferUsageHint.StaticDraw);

            numElements += indicesList.Length;
            return IBO;
		}

    }

    class ContentPipe
    {
        public static Texture2D LoadTexture(string filePath)
        {
            Bitmap bitmap = new Bitmap(filePath);

            int id = GL.GenTexture();

            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0,0,bitmap.Width,bitmap.Height),
                ImageLockMode.ReadOnly,System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.BindTexture(TextureTarget.Texture2D,id);

            GL.TexImage2D(TextureTarget.Texture2D, 0,
                PixelInternalFormat.Rgba,
                bitmap.Width, bitmap.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                bmpData.Scan0);

            bitmap.UnlockBits(bmpData);

            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

            return new Texture2D(bitmap,id,bitmap.Width,bitmap.Height);
        }

        public static Texture2D LoadBmp(string filePath)
        {
            Bitmap bitmap = new Bitmap(filePath);

            return new Texture2D(bitmap,0,bitmap.Width,bitmap.Height);
        }
        
        //public static Color GetPixel(int x, int y)
        //{
        //    if(bitmap != null)
        //    {
        //        return bitmap.GetPixel(x,y);
        //    }
        //    return Color.White;
        //}
    }

    class Texture2D
    {
        public Bitmap bitmap;
        public Mat    mat;
        private bool load;
        private int  id;
        private int  width, height;

        public bool Load  {get { return load;  } }
        public int  ID    {get { return id;    } }
        public int  Width {get { return width; } }
        public int  Height{get { return height;} }

        public Texture2D(Bitmap bitmap = null, int id = 0, int width = 0, int height = 0)
        {
            this.bitmap = bitmap;
            this.id     = id    ;
            this.width  = width ;
            this.height = height;
            if(bitmap != null) this.load   = true ;
            else               this.load   = false;
        }

        public bool LoadMat(Mat mat)
        {
            if(mat == null) return false;

            this.mat = mat.Clone();
            //this.bitmap = (Bitmap)bitmap.Clone();
            this.width  = mat.Width ;
            this.height = mat.Height;
            if(mat != null) this.load   = true ;
            else            this.load   = false;
            return load;
        }

        public bool LoadBmp(Bitmap bitmap)
        {
            if(bitmap == null) return false;

            this.bitmap = (Bitmap)bitmap.Clone();
            this.width  = bitmap.Width ;
            this.height = bitmap.Height;
            if(bitmap != null) this.load   = true ;
            else               this.load   = false;
            return load;
        }

        public Color GetPixelBitmap(int x, int y)
        {
            if(!load) return Color.Black;
            return bitmap.GetPixel(x,y);
        }

        public Color GetPixelMat(int x, int y, int cols, int elementSize)
        {
            if(!load) return Color.Black;
            
            Color color ;
            byte r = 0,g = 0,b = 0 ;  

            int c = cols;//mat.Cols ;
            int e = elementSize;//mat.ElementSize ;
            unsafe
            {
                byte* Data = (byte *)mat.DataPointer;               
                
                if(elementSize == 3)
                {
                    b = *(Data + (y * c + x) * e + 0);
                    g = *(Data + (y * c + x) * e + 1);
                    r = *(Data + (y * c + x) * e + 2);
                    color = Color.FromArgb(r,g,b);
                }
                else
                {
                    r = *(Data + (y * c + x) * e);
                    color = Color.FromArgb(r,r,r);
                }
            }
            return color;
        }

    }
}
