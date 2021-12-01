using UnityEngine;


public class ExpressionShaderAuto : MonoBehaviour
{
	private readonly ExpressionShader m_internals = new ExpressionShader();


	private void Start()
	{
		InvokeRepeating("Randomize", 0.0f, 5.0f);
	}


	public void Randomize()
	{
		// get params
		// TODO: randomize/vary?
		const uint recursionMax = 5;
		const bool discontinuous = false;

		// randomize & update shader
		m_internals.Randomize(recursionMax, discontinuous, null);
	}
}
