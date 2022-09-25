using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HoopTrigger : MonoBehaviour
{
    public Collider bottomCollider;
    private ParticleSystem particles;
    public UnityEvent winEvent;

    private void Awake()
    {
        particles = GetComponent<ParticleSystem>();
    }
    private void OnTriggerEnter(Collider other)
    {
        bottomCollider.enabled = false;
        particles.Play();
        winEvent.Invoke();
    }
}
