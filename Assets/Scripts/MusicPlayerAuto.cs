using System.Collections;
using System.Linq;
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


	public void Start()
	{
		m_player = new MusicPlayer();
		m_player.Start();
	}

	public void Generate(bool isScale)
	{
		const int numExcludedInstruments = 8; // due to the last several standard MIDI instruments being not great for randomization

		// pass input through
		m_player.m_samplesPerSecond = m_samplesPerSecond;
		m_player.m_stereo = m_stereo;
		m_player.m_maxPolyphony = m_maxPolyphony;
		m_player.m_tempo = (uint)Random.Range(45, 90);
		m_player.m_scaleReuse = false;
		m_player.m_rootNoteIndex = (uint)Random.Range((int)(MusicUtility.midiMiddleCKey - MusicUtility.tonesPerOctave), (int)(MusicUtility.midiMiddleCKey + MusicUtility.tonesPerOctave));
		m_player.m_scaleIndex = (uint)Random.Range(0, MusicUtility.scales.Length);
		m_player.m_instrumentToggles = Enumerable.Repeat(true, m_player.InstrumentCount - numExcludedInstruments).Concat(Enumerable.Repeat(false, numExcludedInstruments)).ToArray();
		m_player.m_chordReuse = false;
		m_player.m_rhythmReuse = false;
		m_player.m_harmonyCount = (uint)Random.Range(0, 3);
		m_player.m_instrumentCount = (uint)Random.Range(1, 3);
		m_player.m_volume = 0.01f;
		m_player.m_bankFilePath = m_bankFilePath;

		// generate
		m_player.Generate(isScale);
	}

	public void Play()
	{
		m_player.Play(GetComponent<AudioSource>());
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
			StartCoroutine(GeneratePlayMultiFrame());
		}
	}

	private IEnumerator GeneratePlayMultiFrame()
	{
		yield return null; // to allow a redisplay w/ the new mute icon before the generation delay

		Generate(false);
		yield return null;

		Play();
		m_muteToggle.interactable = true;
		yield return new WaitForSeconds(m_player.LengthSeconds + Random.Range(1.0f, 4.0f)); // NOTE that the perceived delay will often be higher than the added time due to generation delay

		Mute(false);
	}
}
