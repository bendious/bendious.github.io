using System.Collections;
using System.Linq;
#if !UNITY_WEBGL || UNITY_EDITOR
using System.Threading;
#endif
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class MusicPlayerAuto : MonoBehaviour
{
	public uint m_samplesPerSecond = 44100U;
	public bool m_stereo = true;
	public uint m_maxPolyphony = 40U;
	public UnityEngine.UI.Selectable m_muteToggle;

	public string m_bankFilePath = "GM Bank/gm";


	private MusicPlayer m_player;

#if !UNITY_WEBGL || UNITY_EDITOR
	volatile
#endif
		private bool m_generateDone = false;


	public void Start()
	{
		m_player = new MusicPlayer();
		m_player.Start();
	}

	public void Mute(bool enable)
	{
		if (enable)
		{
			StopAllCoroutines();
			GetComponent<AudioSource>().Stop();
			m_muteToggle.interactable = true;
		}
		else
		{
			StartCoroutine(PlayAfterGeneration());
		}
	}


	private void Generate()
	{
		const int numExcludedInstruments = 8; // due to the last several standard MIDI instruments being not great for randomization

		// pass input through
		m_player.m_samplesPerSecond = m_samplesPerSecond;
		m_player.m_stereo = m_stereo;
		m_player.m_maxPolyphony = m_maxPolyphony;
		m_player.m_tempo = (uint)Utility.RandomRange(45, 90);
		m_player.m_scaleReuse = false;
		m_player.m_rootNoteIndex = (uint)Utility.RandomRange((int)(MusicUtility.midiMiddleCKey - MusicUtility.tonesPerOctave), (int)(MusicUtility.midiMiddleCKey + MusicUtility.tonesPerOctave));
		m_player.m_scaleIndex = (uint)Utility.RandomRange(0, MusicUtility.scales.Length);
		m_player.m_instrumentToggles = Enumerable.Repeat(true, m_player.InstrumentCount - numExcludedInstruments).Concat(Enumerable.Repeat(false, numExcludedInstruments)).ToArray();
		m_player.m_chordReuse = false;
		m_player.m_rhythmReuse = false;
		m_player.m_harmonyCount = (uint)Utility.RandomRange(0, 3);
		m_player.m_instrumentCount = (uint)Utility.RandomRange(1, 3);
		m_player.m_volume = 0.01f;
		m_player.m_bankFilePath = m_bankFilePath;

		// generate
		m_player.Generate(false);
		m_generateDone = true;
	}

	private IEnumerator PlayAfterGeneration()
	{
		m_generateDone = false;
#if UNITY_WEBGL && !UNITY_EDITOR
			Thread thread = new Thread(new ThreadStart(Generate));
			thread.IsBackground = true;
			thread.Priority = System.Threading.ThreadPriority.Lowest;
			thread.Start();
#else
		Generate();
#endif

		yield return new WaitUntil(() => m_generateDone);
		m_muteToggle.interactable = true;
		m_player.Play(GetComponent<AudioSource>());

		yield return new WaitForSeconds(m_player.LengthSeconds + Utility.RandomRange(1.0f, 4.0f)); // NOTE that the perceived delay will often be higher than the added time due to subsequent generation delay
		Mute(false);
	}
}
