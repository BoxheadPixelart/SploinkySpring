using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.SploinkySpring
{
    
    [System.Serializable]
    public class SpringData 
    {
        [Min(0)]
        public float damp = 1;
        [Min(0)]
        public float speed = 1;
        [Min(0)]
        public float freq = 1;
        public SpringData(float d, float f, float s)
        {
            damp = d;
            freq = f;
            speed = s;
        }

        public void SetData(float d, float s, float f)
        {
            damp = d;
            speed = s;
            freq = f; 
        }

       
    }
    
    public class Sploinky
    {
        
        public class SpringBase
        {
            public SpringData SpringData = new SpringData(1,1,1);
            public void SetSpringData(SpringData data)
            {
                SpringData = data; 
            }
            public SpringData GetSpringData()
            {
                return SpringData; 
            }
            
        }
        
        
           
        [System.Serializable]
        public class FloatSpring : SpringBase
        {
            public float value;
            public float velocity;
            //
            public float Output => value + velocity;
            public FloatSpring()
            {
                value = 0;
                velocity = 0;
      
            }
            
            public FloatSpring Spring(float goalFloat)
            {
                float[] result = Spring_Float(value, velocity, goalFloat, SpringData.damp, SpringData.freq, SpringData.speed);
                value = result[0];
                velocity = result[1];
                return this;
            }
            
        }
        
        
        [System.Serializable]
        public class Vector3Spring : SpringBase
        {
            public Vector3 value;
            public Vector3 velocity;
            public Vector3 Output => value + velocity;

            public Vector3Spring()
            {
                value = new Vector3();
                velocity = new Vector3();
               
                //SpringData
            }
            public Vector3Spring Spring(Vector3 goalVector)
            {
                Vector3[] result = Spring_Vector3(value, velocity, goalVector, SpringData.damp, SpringData.freq, SpringData.speed);
                value = result[0];
                velocity = result[1];
                return this;
            }
        }

        [System.Serializable]
        public class RotationSpring : SpringBase
        {
            public Vector3 uVal;//up
            public Vector3 uVelo;
            public Vector3 fVal;//forward
            public Vector3 fVelo;
            

            public Vector3 UpOutput => uVal + uVelo;

            public Vector3 ForwardOutput => fVal + fVelo;

            public Quaternion Output => Quaternion.LookRotation(ForwardOutput,UpOutput);


            public RotationSpring()
            {
                uVal = new Vector3();
                uVelo = new Vector3();
                fVal = new Vector3();
                fVelo = new Vector3();


                //
            }
            public RotationSpring Spring(Transform transform)
            {
                Vector3 targetForward = transform.forward; 
                Vector3 targetUp = transform.up;
                Vector3[] forward = Spring_Vector3(fVal, fVelo, targetForward, SpringData.damp, SpringData.freq, SpringData.speed);
                fVal = forward[0];
                fVelo = forward[1];
                
                Vector3[] up = Spring_Vector3(uVal, uVelo, targetUp, SpringData.damp, SpringData.freq, SpringData.speed);
                uVal = up[0];
                uVelo = up[1];
                return this;
            }
        }
        [System.Serializable]
        public class TransformSpring
        {
            public bool[] active = new bool[3];  
            public Vector3Spring position;
            public Vector3Spring scale;
            public RotationSpring rotation;

            public TransformSpring()
            {
                active[0] = true;
                active[1] = true;
                active[2] = true; 
                position = new Vector3Spring();
                scale = new Vector3Spring();
                rotation = new RotationSpring();
            }

            public TransformSpring Spring(Transform transform, Vector3 forwardoffset)
            {
                if (active[0]) position.Spring(transform.position +( transform.forward * forwardoffset.z)  + (transform.up * forwardoffset.y)+transform.right * forwardoffset.x);
                if (active[1]) rotation.Spring(transform);
                if (active[2]) scale.Spring(transform.localScale);

                return this;
            }

            public void SetSpringData(SpringData pos,SpringData rot, SpringData sca)
            {
                position.SetSpringData(pos);
                rotation.SetSpringData(rot);
                scale.SetSpringData(sca);
            }
        }
        
        
        public static float[] Spring_Float(float value, float velocity, float target_value, float damping_ratio, float angular_frequency, float time_step)
        {
            //Math from: http://allenchou.net/2015/04/game-math-more-on-numeric-springing/ and http://allenchou.net/2015/04/game-math-precise-control-over-numeric-springing/
            float[] _ret = {0, 0};
            float _x = value; //Input value
            float _v = velocity * (120 / time_step); //Input velocity
            float _x_t = target_value; //Target value
            float _damping = damping_ratio; //Damping of the oscillation (0 = no damping, 1 = critically damped)
            float _ang_freq = 2 * Mathf.PI * angular_frequency; //Oscillations per second
            float _t = time_step / 120; //How much of a second each step/use of the script takes (1 = normal time, 2 = twice as fast,..)
            float _delta_v, _delta_x, _delta;

            _delta = (1 + 2 * _t * _damping * _ang_freq) + Mathf.Pow(_t, 2) * Mathf.Pow(_ang_freq, 2);
            _delta_x = (1 + 2 * _t * _damping * _ang_freq) * _x + _t * _v + Mathf.Pow(_t, 2) * Mathf.Pow(_ang_freq, 2) * _x_t;
            _delta_v = _v + _t * Mathf.Pow(_ang_freq, 2) * (_x_t - _x);


            _ret[0] = _delta_x / _delta; //Output value
            _ret[1] = (_delta_v / _delta) / (120 / time_step); //Output velocity
            return _ret;
        }
        
        //
        public static float Spring_Float(float value, ref float velocity, float target_value, float damping_ratio, float angular_frequency, float time_step)
        {
            //Math from: http://allenchou.net/2015/04/game-math-more-on-numeric-springing/ and http://allenchou.net/2015/04/game-math-precise-control-over-numeric-springing/
            float _x = value;                        //Input value
            float _v = velocity * (120 / time_step);  //Input velocity
            float _x_t = target_value;                       //Target value
            float _damping = damping_ratio;                  //Damping of the oscillation (0 = no damping, 1 = critically damped)
            float _ang_freq = 2 * Mathf.PI * angular_frequency;             //Oscillations per second
            float _t = time_step / 120;              //How much of a second each step/use of the script takes (1 = normal time, 2 = twice as fast,..)
            float _delta_v, _delta_x, _delta;

            _delta = (1 + 2 * _t * _damping * _ang_freq) + Mathf.Pow(_t, 2) * Mathf.Pow(_ang_freq, 2);
            _delta_x = (1 + 2 * _t * _damping * _ang_freq) * _x + _t * _v + Mathf.Pow(_t, 2) * Mathf.Pow(_ang_freq, 2) * _x_t;
            _delta_v = _v + _t * Mathf.Pow(_ang_freq, 2) * (_x_t - _x);


            var ret = _delta_x / _delta;                        //Output value
            velocity = (_delta_v / _delta) / (120 / time_step); //Output velocity
            return ret;
        }
        ///
        //```
        public static Vector3[] Spring_Vector3(Vector3 value, Vector3 velocity, Vector3 target_value, float damping_ratio, float angular_frequency, float time_step)
        {
            //Math from: http://allenchou.net/2015/04/game-math-more-on-numeric-springing/ and http://allenchou.net/2015/04/game-math-precise-control-over-numeric-springing/
            Vector3[] _ret = { new Vector3(0, 0, 0), new Vector3(0, 0, 0) };

            // X Spring
            float[] x = Spring_Float(value.x, velocity.x, target_value.x, damping_ratio, angular_frequency, time_step);
            //
            float[] y = Spring_Float(value.y, velocity.y, target_value.y, damping_ratio, angular_frequency, time_step);
            //
            float[] z = Spring_Float(value.z, velocity.z, target_value.z, damping_ratio, angular_frequency, time_step);
            //

            _ret[0] = new Vector3(x[0], y[0], z[0]);
            _ret[1] = new Vector3(x[1], y[1], z[1]);
            return _ret;
        }
        public static Vector3 Spring_Vector3(Vector3 value, Vector3 target_value, float damping_ratio, float angular_frequency, float time_step)
        {
            Vector3  velocity = new Vector3(); 
            //Math from: http://allenchou.net/2015/04/game-math-more-on-numeric-springing/ and http://allenchou.net/2015/04/game-math-precise-control-over-numeric-springing/
            Vector3 _ret = new Vector3(0, 0, 0); 

            // X Spring
        
            float[] x = Spring_Float(value.x, velocity.x, target_value.x, damping_ratio, angular_frequency, time_step);
            //
            float[] y = Spring_Float(value.y, velocity.y, target_value.y, damping_ratio, angular_frequency, time_step);
            //
            float[] z = Spring_Float(value.z, velocity.z, target_value.z, damping_ratio, angular_frequency, time_step);
            //

            _ret = new Vector3(x[0], y[0], z[0]);
            velocity = new Vector3(x[1], y[1], z[1]);
            return _ret;
        }
       
        //  public static float[] Spring_RotationAngle(Vector3 value, Vector3 velocity, Vector3 target_value, float damping_ratio, float angular_frequency, float time_step)
        //  {
        //  float RotDiff;
        // float[] _ret = {0,0}; 
        // return _ret;
        // }
        // public static Transform[] Spring_Transform(Vector3 value, Vector3 velocity, Vector3 target_value, float damping_ratio, float angular_frequency, float time_step)
        // {
        //Transform
        //return _ret; 
        //  }
        //

        //

        //
        public static Vector3[] Spring_RotationDirection(Vector3 value, Vector3 velocity, Vector3 target_value, float damping_ratio, float angular_frequency, float time_step)
        {
            Vector3[] ret = { new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
            //
            var xRotDiff = value.x - Mathf.DeltaAngle(target_value.x, value.x);
            float[] xrot = Spring_Float(value.x, velocity.x, xRotDiff, damping_ratio, angular_frequency, time_step);
            //
            var yRotDiff = value.y - Mathf.DeltaAngle(target_value.y, value.y);
            float[] yrot = Spring_Float(value.y, velocity.y, yRotDiff, damping_ratio, angular_frequency, time_step);
            //
            var zRotDiff = value.z - Mathf.DeltaAngle(target_value.z, value.z);
            float[] zrot = Spring_Float(value.z, velocity.z, zRotDiff, damping_ratio, angular_frequency, time_step);
            //
            ret[0] = new Vector3(xrot[0], yrot[0], zrot[0]);
            ret[1] = new Vector3(xrot[1], yrot[1], zrot[1]);
            //
            return ret;
        }
        
        
        
        public static Quaternion ShortestRotation(Quaternion a, Quaternion b)
        {
            if (Quaternion.Dot(a, b) < 0)
            {
                return a * Quaternion.Inverse(Multiply(b, -1));
            }
            else return a * Quaternion.Inverse(b);
        }



        public static Quaternion Multiply(Quaternion input, float scalar) 
        {
            return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
        }
        
        
    }
    
    
    //  public static float[] Spring_RotationAngle(Vector3 value, Vector3 velocity, Vector3 target_value, float damping_ratio, float angular_frequency, float time_step)
    //  {
    //  float RotDiff;
    // float[] _ret = {0,0}; 
    // return _ret;
    // }
    // public static Transform[] Spring_Transform(Vector3 value, Vector3 velocity, Vector3 target_value, float damping_ratio, float angular_frequency, float time_step)
    // {
    //Transform
    //return _ret; 
    //  }
    //

    //

    //

}

