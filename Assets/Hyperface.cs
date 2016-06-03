﻿using UnityEngine;
using System.Collections;
using System;

namespace Holojam.IO {
    public class Hyperface : ViveGlobalReceiver, IGlobalTriggerPressSetHandler , IGlobalTouchpadPressUpHandler {
        Vector3 A_;
        Vector3 B_;
        Vector4 A, B;
        public Transform box;
        bool isbutton;
        Trackball trackball;
        float radius;
        public int[] hyperface;
        public HyperCubeMesh hypermesh;
        // Use this for initialization
        public Vector3[] vertices;
        public int faceindex;
        public int[] faces;
        public Mesh mesh;
        public GameObject parent;
        void Start() {
            faceindex = 0;
            trackball = new Trackball(4);
            A_ = new Vector3();
            B_ = new Vector3();
            isbutton = false;
            A = new Vector4();
            B = new Vector4();
            radius = 1.0f;
            hypermesh = new HyperCubeMesh();
            vertices = new Vector3[8];
            hyperface = new int[] {1,3,5,7,9,11,13,15,
                                          0,2,4,6,8,10,12,14,
                                          2,3,6,7,10,11,14,15,
                                          0,1,4,5,8,9,12,13,
                                          4,5,6,7,12,13,14,15,
                                          0,1,2,3,8,9,10,11,
                                          8,9,10,11,12,13,14,15,
                                          0,1,2,3,4,5,6,7};
            for (int i = 0; i < 8; i++) {
                vertices[i] = hypermesh.get3dver(hyperface[i + faceindex * 8]);
            }

            mesh = new Mesh();
            faces = new int[24] {
               2,0,4,6,
               3,1,5,7,
               1,0,4,5,
               3,2,6,7,
               1,0,2,3,
               5,4,6,7
            };
            mesh.vertices = vertices;
            mesh.SetIndices(faces, MeshTopology.Quads, 0);
            mesh.RecalculateBounds();
            mesh.Optimize();
            GetComponent<MeshFilter>().mesh = mesh;

        }
        void UpdateRotation(HyperCubeMesh cube, Trackball trackball, Vector4 A_, Vector4 B_) {

            float[] A = new float[4] { A_.x, A_.y, A_.z, A_.w };
            float[] B = new float[4] { B_.x, B_.y, B_.z, B_.w };

            trackball.rotate(A, B);

            for (int i = 0; i < 16; i++) {

                float[] src = new float[4];
                src[0] = cube.srcVertices[i].x;
                src[1] = cube.srcVertices[i].y;
                src[2] = cube.srcVertices[i].z;
                src[3] = cube.srcVertices[i].w;
                float[] dst = new float[4];

                trackball.transform(src, dst);

                cube.updatepoint4(dst, i);
            }

        }
        // Update is called once per frame
        void Update() {
            for (int i = 0; i < 8; i++) {
                vertices[i] = hypermesh.get3dver(hyperface[i + faceindex * 8]);
            }

            if (isbutton) {
                Vector3 relapos = new Vector3();
                relapos = (B_ - box.position) * 8f / 3f;
                float r = (float)Math.Sqrt(relapos.x * relapos.x + relapos.y * relapos.y + relapos.z * relapos.z);
                if (r < radius) {
                    B = new Vector4(relapos.x, relapos.y, relapos.z, (float)Math.Sqrt(radius * radius - relapos.x * relapos.x - relapos.y * relapos.y - relapos.z * relapos.z));
                } else {
                    //float length = relapos.magnitude;
                    Vector3 Q = (radius / r) * relapos;
                    relapos = Q + box.position;
                    B = new Vector4(Q.x, Q.y, Q.z, 0f);
                }
                UpdateRotation(hypermesh, trackball, A, B);
                A = B;

            }


            mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.SetIndices(faces, MeshTopology.Quads, 0);
            mesh.RecalculateBounds();
            mesh.Optimize();
            GetComponent<MeshFilter>().mesh = mesh;
        }
        public void OnGlobalTriggerPressDown(ViveEventData eventData) {
            isbutton = true;
            B_ = eventData.module.transform.position;
            Vector3 relapos = new Vector3();
            relapos = (B_ - box.position) * 8f / 3f;
            float r = (float)Math.Sqrt(relapos.x * relapos.x + relapos.y * relapos.y + relapos.z * relapos.z);
            if (r < radius) {
                B = new Vector4(relapos.x, relapos.y, relapos.z, (float)Math.Sqrt(radius * radius - relapos.x * relapos.x - relapos.y * relapos.y - relapos.z * relapos.z));
            } else {
                Vector3 Q = (radius / r) * relapos;
                B = new Vector4(Q.x, Q.y, Q.z, 0f);
            }
            A = B;
            // Debug.Log("Trigger Pressed Down");
        }

        public void OnGlobalTriggerPress(ViveEventData eventData) {
            isbutton = true;
            B_ = eventData.module.transform.position;
            // Debug.Log("Trigger Pressed ");
        }

        public void OnGlobalTriggerPressUp(ViveEventData eventData) {
            isbutton = false;
            A_ = eventData.module.transform.position;
            B_ = eventData.module.transform.position;
            // Debug.Log("Trigger Pressed up");
        }

        public void OnGlobalTriggerTouchDown(ViveEventData eventData) {
            throw new NotImplementedException();
        }

        public void OnGlobalTriggerTouch(ViveEventData eventData) {
            throw new NotImplementedException();
        }

        public void OnGlobalTriggerTouchUp(ViveEventData eventData) {
            throw new NotImplementedException();
        }

        public void OnGlobalTouchpadPressUp(ViveEventData eventData) {
            faceindex = (faceindex + 1) % 8;
        }
    }
}
