using TMPro;
using UnityEngine;

public class DebuggerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textInfo;
    [SerializeField] private bool _showDebugText;

    private void Update()
    {
        if (_showDebugText)
            _textInfo.text = Debugger.WholeTextInfo;
    }
}
