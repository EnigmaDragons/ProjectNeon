using UnityEngine;

namespace AeLa.EasyFeedback.Utility
{
	internal static class ExtensionMethods
	{
		/// <summary>
		/// Wraps a class around a json array so that it can be deserialized by JsonUtility
		/// </summary>
		/// <param name="source"></param>
		/// <param name="topClass"></param>
		/// <returns></returns>
		public static string WrapToClass(this string source, string topClass) =>
			$"{{\"{topClass}\": {source}}}";

		/// <summary>
		/// Scales the texture by the provided amount.
		/// </summary>
		/// <returns></returns>
		public static void Scale(this Texture2D tex, float scale, FilterMode filterMode = FilterMode.Trilinear, bool updateMipMaps = false)
		{
			var width = Mathf.RoundToInt(tex.width * scale);
			var height = Mathf.RoundToInt(tex.height * scale);

			// prepare texture
			tex.filterMode = filterMode;
			tex.Apply(updateMipMaps);

			// scale texture using the gpu
			var rt = new RenderTexture(width, height, 32);
			Graphics.SetRenderTarget(rt);
			GL.LoadPixelMatrix(0, 1, 1, 0);
			GL.Clear(true, true, default);
			Graphics.DrawTexture(new Rect(0, 0, 1, 1), tex);

			// apply to texture
			tex.Resize(width, height);
			tex.ReadPixels(new Rect(0, 0, width, height), 0, 0, updateMipMaps);
			tex.Apply(updateMipMaps);
		}
	}
}