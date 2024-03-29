using UnityEngine;
using UnityEngine.Events;

public class Drawer : MonoBehaviour
{
    [SerializeField] private LayerMask _ignoreLayer;
    [SerializeField] private UnityEvent OnStartDraw;
    [SerializeField] private UnityEvent OnEndDraw;
    private Camera _camera;
    private Line _currentLine;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        ToggleState();
    }

    private void ToggleState()
    {
        if (Input.touchCount == 0)
            return;
        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                InstantiateLine(touch.position);
                break;
            case TouchPhase.Moved:
                DrawLine(touch.position);
                break;
            case TouchPhase.Ended:
                FinishDrawing(touch.position);
                break;
        }
    }

    private void FinishDrawing(Vector2 touchPosition)
    {
        if (_currentLine == null)
            return;
        if (!IsTouchDrawingPoint(touchPosition, out EndDrawingPoint drawingPoint) || (_currentLine.Party != drawingPoint.Party && drawingPoint.Party != Party.Universal))
            Destroy(_currentLine.gameObject);
        else
            OnEndDraw?.Invoke();
        _currentLine = null;
    }

    private void InstantiateLine(Vector2 touchPosition)
    {
       if (IsTouchDrawingPoint(touchPosition, out StartDrawingPoint drawingPoint) && drawingPoint.GetLine() == null)
        {
            _currentLine = drawingPoint.InstantiateLine(_camera.ScreenToWorldPoint(touchPosition));
            OnStartDraw?.Invoke();
        }
    }

    private bool IsTouchDrawingPoint<T>(Vector2 touchPosition, out T drawingPoint) where T : MonoBehaviour
    {
        drawingPoint = null;
        RaycastHit2D hit = Physics2D.Raycast(_camera.ScreenToWorldPoint(touchPosition), Vector3.back, 100, ~_ignoreLayer);
        return (hit.transform != null && hit.transform.gameObject.TryGetComponent(out drawingPoint));
    }

    private void DrawLine(Vector2 touchPosition)
    {
        if (_currentLine == null)
            return;
        _currentLine.DrawSegment(_camera.ScreenToWorldPoint(touchPosition));
    }
}
