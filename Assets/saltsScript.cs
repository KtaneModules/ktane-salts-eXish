using System;
using System.Collections;
using System.Text.RegularExpressions;
using KModkit;
using KTMissionGetter;
using UnityEngine;

public class saltsScript : MonoBehaviour
{
	private void Start()
	{
		if (Mission.ID == MISSION_ID)
		{
			_isTFA = true;
			ghost.material.mainTexture = tweet;
			Debug.LogFormat("[Salts #{0}] This Salts is secretly Twitter. It will solve automatically at the end of the other bomb.", new object[]
			{
				_id
			});
			return;
		}
		int num = 0;
		foreach (int num2 in GetComponent<KMBombInfo>().GetSerialNumberNumbers())
		{
			num += num2;
		}
		answer1 = num / 5;
		answer2 = num % 5;
		Debug.LogFormat("[Salts #{0}] Tap the ghost {1} times, then {2} times.", new object[]
		{
			_id,
			answer1 + 1,
			answer2 + 1
		});
		KMSelectable kmselectable = button;
		kmselectable.OnInteract = (KMSelectable.OnInteractHandler)Delegate.Combine(kmselectable.OnInteract, new KMSelectable.OnInteractHandler(Press));
	}

	private bool Press()
	{
		if (_isTFA)
		{
			return false;
		}
		GetComponent<KMSelectable>().AddInteractionPunch(0.1f);
		if (waiting != null)
		{
			StopCoroutine(waiting);
		}
		waiting = StartCoroutine(Wait());
		count++;
		return false;
	}

	private IEnumerator Wait()
	{
		yield return new WaitForSeconds(0.5f);
		if (state == 0)
		{
			GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonRelease, transform);
			if (count == answer1 + 1)
			{
				state = 1;
				Debug.LogFormat("[Salts #{0}] Correct first set of taps.", new object[]
				{
					_id
				});
			}
			else
			{
				GetComponent<KMBombModule>().HandleStrike();
				Debug.LogFormat("[Salts #{0}] You tapped {1} times. Strike!", new object[]
				{
					_id,
					count
				});
			}
		}
		else if (state == 1)
		{
			if (count == answer2 + 1)
			{
				state = 2;
				Debug.LogFormat("[Salts #{0}] Correct second set of taps. Solved!", new object[]
				{
					_id
				});
				GetComponent<KMBombModule>().HandlePass();
				GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
				foreach (TriangleExplosion triangleExplosion in GetComponentsInChildren<TriangleExplosion>())
				{
					triangleExplosion.StartCoroutine(triangleExplosion.SplitMesh(false));
				}
			}
			else
			{
				Debug.LogFormat("[Salts #{0}] You tapped {1} times. Strike!", new object[]
				{
					_id,
					count
				});
				GetComponent<KMBombModule>().HandleStrike();
			}
		}
		count = 0;
		waiting = null;
		yield break;
	}

	private IEnumerator ProcessTwitchCommand(string command)
	{
		if (_isTFA)
		{
			yield return "sendtochat Tweet tweet! I am Twitter and you cannot stop me!";
			yield break;
		}
		Match i;
		if ((i = Regex.Match(command, "^\\s*blan\\s*jumpscare\\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
		{
			yield return null;
			ghost.material.mainTexture = blan;
		}
		if ((i = Regex.Match(command, "^\\s*(?:(?:press|tap|push|ghost|go)\\s*)?(\\d\\d?)\\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)).Success)
		{
			yield return null;
			int c = int.Parse(i.Groups[1].Value);
			for (int j = 0; j < c; j++)
			{
				button.OnInteract.Invoke();
				yield return new WaitForSeconds(0.1f);
			}
			if ((state == 0 && count != (answer1 + 1)) || (state == 1 && count != (answer2 + 1)))
				yield return "strike";
			else if (state == 1)
				yield return "solve";
		}
		yield break;
	}

	private IEnumerator TwitchHandleForcedSolve()
	{
		if (_isTFA)
		{
			while (true)
			{
				yield return true;
			}
		}
		if (waiting != null)
        {
			if ((state == 0 && count > (answer1 + 1)) || (state == 1 && count > (answer2 + 1)))
            {
				state = 2;
				GetComponent<KMBombModule>().HandlePass();
				yield break;
			}
		}
		if (state == 0)
		{
			int start = count;
			for (int i = start; i < answer1 + 1; i++)
			{
				button.OnInteract.Invoke();
				yield return new WaitForSeconds(0.1f);
			}
			while (waiting != null)
			{
				yield return true;
			}
		}
		if (state == 1)
		{
			int start = count;
			for (int j = start; j < answer2 + 1; j++)
			{
				button.OnInteract.Invoke();
				yield return new WaitForSeconds(0.1f);
			}
			while (waiting != null)
			{
				yield return true;
			}
		}
		yield break;
	}

	private int answer1 = -1;

	private int answer2 = -1;

	private int state;

	private int count;

	public KMSelectable button;

	public Texture blan;

	public Texture tweet;

	public Renderer ghost;

	private Coroutine waiting;

	private int _id = ++_idc;

	private static int _idc;

	private const string MISSION_ID = "mod_twitterForAndroid_tfa";

	private bool _isTFA;

	private readonly string TwitchHelpMessage = "\"!{0} tap 3\" taps the module 3 times.";
}