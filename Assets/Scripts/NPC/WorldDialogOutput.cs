using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WorldDialogOutput : MonoBehaviour {

    private Text spokenText;
    private AudioSource source;

    [SerializeField]
    private List<Line> outputQueue = new List<Line>();

    void Awake()
    {
        spokenText = GetComponent<Text>();
        source = transform.root.GetComponent<AudioSource>();

        if (!spokenText)
        {
            Debug.LogError("outputField missing");
        }
    }

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(Speaker());
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void SpeakText(Line line)
    {
        outputQueue.Add(line);
        // Debug.Log("get line");
    }

    IEnumerator Speaker()
    {
        for(;;)
        {
            yield return new WaitForSeconds(1f);

            // has no text to speak?
            if (outputQueue.Count == 0)
            {
                // wait for text
                continue;
            }
            else
            {
                // Debug.Log("line to speak: " + outputQueue[0]);

                // get line
                Line dialogPart = outputQueue[0];

                // speak line
                for (int i = 0; i < dialogPart.lineTexts.Count; i++)
                {
                    LinePart line = dialogPart.lineTexts[i];

                    // add line to spokenText
                    // Debug.Log("speak line");
                    spokenText.text = line.text;

                    // start audio clip
                    if (line.audio != null)
                    {
                        yield return StartCoroutine(PlayAudioClip(line.audio, source));

                        // camera focus speaker
                    }
                    // beta wait for next
                    else
                    {
                        yield return StartCoroutine(WaitForNextLine(line.text));
                    }

                    spokenText.text = "";
                }

                // Debug.Log("speak finished");

                // remove line from queue
                outputQueue.RemoveAt(0);
            }
        }
    }

    IEnumerator PlayAudioClip(AudioClip clip, AudioSource source)
    {
        // add clip to avatar
        source.clip = clip;

        // start audio
        source.Play();

        //wait for end of clip
        while (source.isPlaying)
        {
            yield return null;
        }
    }

    IEnumerator WaitForNextLine(string line)
    {
        float startTime = Time.time;
        float waitTime = line.Length * 0.2f;
        yield return new WaitForSeconds(waitTime);
    }
}
