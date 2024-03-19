using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    public Transform orietation;
    public Transform playerCam;
    private Rigidbody rb;
    private PlayerMovement pm;
    public Slider sliderCd;
    public Animator animator;

    [Header("Dashing")]
    public float dashForce;
    public float dashUpwardForce;
    public float dashDuration;

    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    [Header("Input")]
    public KeyCode dashKey = KeyCode.E;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Actualizar el temporizador de cooldown
        dashCdTimer -= Time.deltaTime;
        sliderCd.value = dashCdTimer;

        if (Input.GetKeyDown(dashKey) && dashCdTimer <= 0f)
        {
            Dash();
        }
    }

    void Dash()
    {
        pm.particles3.SetActive(true);
        pm.dashing = true;
        Invoke("ResetParticles", 1f);
        // Aplicar fuerza solo si el cooldown ha terminado
        if (dashCdTimer <= 0f)
        {
            Vector3 forceToApply = orietation.forward * dashForce + orietation.up * dashUpwardForce;
            rb.AddForce(forceToApply, ForceMode.Impulse);

            // Iniciar el temporizador de cooldown
            dashCdTimer = dashCd;

            // Programar la llamada para restablecer el dash después de la duración del dash
            Invoke(nameof(ResetDash), dashDuration);
            
        }
    }

    void ResetDash()
    {
        pm.dashing = false;
        animator.SetBool("Dash", false);
        
    }

    public void ResetParticles()
    {
        pm.particles3.SetActive(false);
    }
}

