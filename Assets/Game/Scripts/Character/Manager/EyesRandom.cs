using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesRandom : StateMachineBehaviour
{
  
  
   [SerializeField] private float _numberOfAnimations;
    private float _idleTime;
    private bool _isIdle = true; // Commence par une phase d'inactivité
     
     private float _nextAnimationTime =2f; // Temps avant la prochaine animation

 
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _idleTime = 0; // Réinitialiser le compteur de temps à l'entrée de l'état        
    }

    // OnStateUpdate est appelé à chaque frame pendant que l'état est actif
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _idleTime += Time.deltaTime;

        if (_isIdle && _idleTime > _nextAnimationTime)
        {
            
            int random = Random.Range(1, (int)_numberOfAnimations + 1);
            Debug.Log("Random : " + random);
            animator.SetFloat("EyesAnimation", random);
            _idleTime = 0; 
            _isIdle = false;  // On est plus en phase d'inactivité

            //on choisit un temps aléatoire pour la prochaine animation
            _nextAnimationTime = Random.Range(0.7f, 3f);
        }
        else if (!_isIdle && _idleTime > animator.GetCurrentAnimatorStateInfo(0).length) // Si l'animation est terminée
        {
            animator.SetFloat("EyesAnimation", 0); // Revenir à une animation "idle" ou à une pose neutre si c'est ce que représente l'indice 0
            _idleTime = 0; 
            _isIdle = true; // Revenir à la phase d'inactivité
        }
    }

}
    

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

