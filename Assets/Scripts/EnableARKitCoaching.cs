using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARKit;
using UnityEngine.XR.ARSubsystems;

/// <summary>Enables ARKit's automated coaching guide when run on supported devices (iOS).</summary>
///
/// <seealso href="https://zenn.dev/katopan/articles/4a655f0eb07c22">Unity ARKitのARCoachingOverlayを表示する｜Mizuki</seealso>
[RequireComponent(typeof(ARSession))]
public class EnableARKitCoaching : MonoBehaviour
{
    private ARSession session;

    void Start() {
	session = GetComponent<ARSession>();
	ARSession.stateChanged += OnStateChanged;
    }

    void OnStateChanged(ARSessionStateChangedEventArgs arg) {
	if (arg.state == ARSessionState.SessionInitializing) {
	    if (session.subsystem is ARKitSessionSubsystem arkitSession) {
		arkitSession.requestedCoachingGoal = ARCoachingGoal.VerticalPlane;
		arkitSession.coachingActivatesAutomatically = true;
	    }
	}
    }
}
