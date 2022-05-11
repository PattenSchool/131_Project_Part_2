/*
 * Copyright (c) 2019 Razeware LLC
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish,
 * distribute, sublicense, create a derivative work, and/or sell copies of the
 * Software in any work that is designed, intended, or marketed for pedagogical or
 * instructional purposes related to programming, coding, application development,
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works,
 * or sale is expressly withheld.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is attached to all collectable game objects.
// When the game object is held, it is made a child of the player and it's position is fixed infront of the player.
// We deactivate gravity and it's collider to ensure it does not react with the physical environment.

public class HeldScript : MonoBehaviour
{
    private Rigidbody myRigidBody;
    private Collider myCollider;

    private void Update()
    {
        gameObject.transform.localPosition = new Vector3(0.0f, -0.12f, 1.0f);
    }

    private void OnEnable()
    {
        myRigidBody = gameObject.GetComponent<Rigidbody>();
        myCollider = gameObject.GetComponent<Collider>();
        myRigidBody.useGravity = false;
        myCollider.enabled = false;
    }

    private void OnDisable()
    {
        myRigidBody.useGravity = true;
        myCollider.enabled = true;
        gameObject.transform.parent = null;
    }
}