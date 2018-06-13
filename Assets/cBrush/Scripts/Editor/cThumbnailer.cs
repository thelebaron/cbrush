using System.IO;
using UnityEngine;

namespace cBrush
{
    public static class cBrushThumbnailer
    {
        private static Camera cam;
        private static GameObject camGo;
        private static GameObject tmpGo;

        static int width = 400;
        static int height = 400;
        private static GameObject myThumbnailObject;

        public static void CreateThumbnail(GameObject g)
        {
            var sqr = width;

            myThumbnailObject = g;
            //(GameObject)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/fern.prefab", typeof(GameObject));

            //set game object's attribs
            //====================================================================
            if (tmpGo == null)
                tmpGo = Object.Instantiate(myThumbnailObject) as GameObject;
            tmpGo.name = g.name;

            if (camGo == null)
                camGo = new GameObject();
            camGo.name = "camgo";


            camGo.transform.position = new Vector3(0.0f, 0.0f, -5f);
            tmpGo.transform.position = new Vector3(0f, -0.5f, 0f);
            tmpGo.hideFlags = HideFlags.DontSave;
            camGo.hideFlags = HideFlags.DontSave;


            cam = camGo.GetComponent<Camera>();
            if (cam == null)
                cam = camGo.AddComponent<Camera>();

            cam.clearFlags = CameraClearFlags.Color;
            cam.backgroundColor = Color.grey;
            cam.enabled = false;
            cam.orthographic = false;
            cam.fieldOfView = 15;
            cam.orthographicSize =
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().orthographicSize;
            cam.targetTexture = new RenderTexture((int) width, (int) height, 24, RenderTextureFormat.ARGB32);
            cam.Render();
            RenderTexture.active = cam.targetTexture;

            Texture2D scr = new Texture2D(sqr, sqr, TextureFormat.RGB24, false);
            // false, meaning no need for mipmaps
            scr.ReadPixels(new Rect(0, 0, sqr, sqr), 0, 0);
            RenderTexture.active = null; //can help avoid errors 
            cam.targetTexture = null;
            // consider ... Destroy(tempRT);

            byte[] bytes;
            bytes = scr.EncodeToPNG();

            File.WriteAllBytes(Application.dataPath + "/cBrush/Resources/ThumbnailsGenerated/" + tmpGo.name + ".png",
                bytes);

            Object.DestroyImmediate(camGo);
            Object.DestroyImmediate(tmpGo);
        }
    }
}