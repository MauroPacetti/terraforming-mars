using UnityEngine;

public class ClickToCommandRover : MonoBehaviour
{
    public Camera viewCam;
    public RoverClickToMove rover;
    public LayerMask groundMask = ~0;

    void Reset()
    {
        viewCam = Camera.main;
    }

    void Update()
    {
        if (viewCam == null || rover == null) return;

        // Mouse click (desktop). In VR lo sostituisci con XR Ray Interactor.
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = viewCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 5000f, groundMask))
            {
                rover.SetTarget(hit.point);
            }
        }
    }
}
