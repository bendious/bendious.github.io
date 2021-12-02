using System.Collections;
using UnityEngine;


public class ExpressionShaderAuto : MonoBehaviour
{
	private readonly ExpressionShader m_internals = new ExpressionShader();

	private Coroutine m_randomizationCoroutine;
	private float m_randomizeTimeLast;
	private float m_pauseSecondsElapsed = float.MaxValue;

	private const float m_randomizationSeconds = 5.0f; // TODO: vary?


	private void Start()
	{
		Pause(false);
	}


	public void Pause(bool enable)
	{
		if (enable)
		{
			m_pauseSecondsElapsed = Time.time - m_randomizeTimeLast;
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

		// if resuming after a pause, wait before first randomization
		if (m_pauseSecondsElapsed < m_randomizationSeconds)
		{
			yield return new WaitForSeconds(m_randomizationSeconds - m_pauseSecondsElapsed);
		}

		while (true)
		{
			// randomize & update shader
			m_internals.Randomize(recursionMax, discontinuous, null);
			m_randomizeTimeLast = Time.time;
			yield return new WaitForSeconds(m_randomizationSeconds);
		}
	}
}
