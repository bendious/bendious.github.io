using System;
using System.Runtime.InteropServices;
using UnityEngine;


public class RuntimeShader : MonoBehaviour
{
#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport("RuntimeShader")]
#endif
	private static extern void RegisterPlugin();

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport("RuntimeShader")]
#endif
	static extern IntPtr Execute();

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport("RuntimeShader")]
#endif
	static extern void SetTime(float time);

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
	[DllImport ("__Internal")]
#else
	[DllImport("RuntimeShader")]
#endif
	static extern bool UpdateGLShader([MarshalAs(UnmanagedType.LPStr)] string pSrcDataVert, [MarshalAs(UnmanagedType.LPStr)] string pSrcDataFrag);


	private bool m_shaderReady = false;

	private float m_timeCurrent = 0.0f;


	public void UpdateShader(string srcDataVert, string srcDataFrag)
	{
		try
		{
			m_shaderReady = UpdateGLShader(srcDataVert, srcDataFrag);
		}
		catch (Exception) { m_shaderReady = false; }
	}

	public bool Paused { get; set; }

	public void ResetTime()
	{
		m_timeCurrent = 0.0f;
	}


	void Start()
	{
		RegisterPlugin();
	}

	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination);
		if (m_shaderReady)
		{
			if (!Paused)
			{
				m_timeCurrent += Time.deltaTime;
				SetTime(m_timeCurrent);
			}
			GL.IssuePluginEvent(Execute(), 1);
		}
	}
}
