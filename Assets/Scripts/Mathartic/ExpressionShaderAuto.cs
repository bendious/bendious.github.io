using System.Collections;
using UnityEngine;


public class ExpressionShaderAuto : MonoBehaviour
{
	private readonly ExpressionShader m_internals = new ExpressionShader();

	private Coroutine m_randomizationCoroutine;


	private void Start()
	{
		Pause(false);
	}


	public void Pause(bool enable)
	{
		if (enable)
		{
			StopCoroutine(m_randomizationCoroutine);
		}
		else
		{
			m_randomizationCoroutine = StartCoroutine(RandomizeRepeating());
		}
	}


	private IEnumerator RandomizeRepeating()
	{
		// get params
		// TODO: randomize/vary?
		const uint recursionMax = 5;
		const bool discontinuous = false;

		while (true)
		{
			// randomize & update shader
			m_internals.Randomize(recursionMax, discontinuous, null);
			yield return new WaitForSeconds(5.0f);
		}
	}
}
