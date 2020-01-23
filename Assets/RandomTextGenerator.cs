using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class RandomTextGenerator : MonoBehaviour
{
    Text _displayText;
    public int maxAmountOfChars = 15;
    public int maxSize = 25;
    public int minSize = 25;
    private int _length = 0;
    // Start is called before the first frame update
    void Start()
    {
        _displayText = GetComponent<Text>();
        _length = Data.Alphabet.Length;
    }

    // Update is called once per frame
    void Update()
    {
        _displayText.text = "";
        var chars = Random.Range(1, maxAmountOfChars);
        for (int i = 0; i < chars; i++)
        {
            var c = Random.Range(0, _length);
            var s = Random.Range(minSize, maxSize);
            _displayText.text += Data.Alphabet[c];
            _displayText.fontSize = s;
        }
    }
}
