using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMove : MonoBehaviour
{
    [SerializeField]
    private float _speed;
    [SerializeField]
    private int _startIndex;
    [SerializeField]
    private int _endIndex;
    [SerializeField]
    private Transform[] _sprites;

    private float _viewHeight;
    private void Awake()
    {
        _sprites = new Transform[transform.childCount];
        for(int i =0; i<transform.childCount; i++)
        {
            _sprites[i] = transform.GetChild(i);
        }
        _viewHeight = Camera.main.orthographicSize * 2;
    }
    private void Update()
    {
        Vector3 curPos = transform.position;
        Vector3 nextPos = Vector3.down * _speed * Time.deltaTime;
        transform.position = curPos + nextPos;
        if (_sprites[_endIndex].position.y < -_viewHeight)
        {
            Vector3 backSpritePos = _sprites[_startIndex].localPosition;
            Vector3 frontSpritePos= _sprites[_endIndex].localPosition;
            _sprites[_endIndex].transform.localPosition = new Vector3(_sprites[_endIndex].transform.position.x, (backSpritePos + Vector3.up * _viewHeight).y);

            int startIndexSave = _startIndex;
            _startIndex = _endIndex;
            _endIndex = startIndexSave-1==-1 ? _sprites.Length-1: startIndexSave-1;
        }
    }
}
