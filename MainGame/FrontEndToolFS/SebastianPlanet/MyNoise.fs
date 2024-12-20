namespace FrontEndToolFS.SebastianPlanet

open Godot

(*
This file is part of libnoise-dotnet.
libnoise-dotnet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.
 
libnoise-dotnet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.
 
You should have received a copy of the GNU Lesser General Public License
along with libnoise-dotnet.  If not, see <http://www.gnu.org/licenses/>.
 
Simplex Noise in 2D, 3D and 4D. Based on the example code of this paper:
http://staffwww.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf
 
From Stefan Gustavson, Linkping University, Sweden (stegu at itn dot liu dot se)
From Karsten Schmidt (slight optimizations & restructuring)
 
Some changes by Sebastian Lague for use in a tutorial series.
Some changes by ZeromaXHe for use in my Godot F# project.
 *)

(*
 * Noise module that outputs 3-dimensional Simplex Perlin noise.
 * This algorithm has a computational cost of O(n+1) where n is the dimension.
 * 
 * This noise module outputs values that usually range from
 * -1.0 to +1.0, but there are no guarantees that all output values will exist within that range.
*)
type MyNoise(seed: int) =
    /// Initial permutation table
    // Rider 这个 F# 自动格式化数组分号有点烦……
    // let Source = [| 151; 160; 137; 91; 90; 15; 131; 13; 201; 95; 96; 53; 194; 233; 7; 225; 140; 36; 103; 30; 69; 142;
    //     8; 99; 37; 240; 21; 10; 23; 190; 6; 148; 247; 120; 234; 75; 0; 26; 197; 62; 94; 252; 219; 203;
    //     117; 35; 11; 32; 57; 177; 33; 88; 237; 149; 56; 87; 174; 20; 125; 136; 171; 168; 68; 175; 74; 165;
    //     71; 134; 139; 48; 27; 166; 77; 146; 158; 231; 83; 111; 229; 122; 60; 211; 133; 230; 220; 105; 92; 41;
    //     55; 46; 245; 40; 244; 102; 143; 54; 65; 25; 63; 161; 1; 216; 80; 73; 209; 76; 132; 187; 208; 89;
    //     18; 169; 200; 196; 135; 130; 116; 188; 159; 86; 164; 100; 109; 198; 173; 186; 3; 64; 52; 217; 226; 250;
    //     124; 123; 5; 202; 38; 147; 118; 126; 255; 82; 85; 212; 207; 206; 59; 227; 47; 16; 58; 17; 182; 189;
    //     28; 42; 223; 183; 170; 213; 119; 248; 152; 2; 44; 154; 163; 70; 221; 153; 101; 155; 167; 43; 172; 9;
    //     129; 22; 39; 253; 19; 98; 108; 110; 79; 113; 224; 232; 178; 185; 112; 104; 218; 246; 97; 228; 251; 34;
    //     242; 193; 238; 210; 144; 12; 191; 179; 162; 241; 81; 51; 145; 235; 249; 14; 239; 107; 49; 192; 214; 31;
    //     181; 199; 106; 157; 184; 84; 204; 176; 115; 121; 50; 45; 127; 4; 150; 254; 138; 236; 205; 93; 222; 114;
    //     67; 29; 24; 72; 243; 141; 128; 195; 78; 66; 215; 61; 156; 180 |]
    let Source = [| 151; 160; 137; 91; 90; 15; 131; 13; 201; 95; 96; 53; 194; 233; 7; 225; 140; 36; 103; 30; 69; 142;
        8; 99; 37; 240; 21; 10; 23; 190; 6; 148; 247; 120; 234; 75; 0; 26; 197; 62; 94; 252; 219; 203;
        117; 35; 11; 32; 57; 177; 33; 88; 237; 149; 56; 87; 174; 20; 125; 136; 171; 168; 68; 175; 74; 165;
        71; 134; 139; 48; 27; 166; 77; 146; 158; 231; 83; 111; 229; 122; 60; 211; 133; 230; 220; 105; 92; 41;
        55; 46; 245; 40; 244; 102; 143; 54; 65; 25; 63; 161; 1; 216; 80; 73; 209; 76; 132; 187; 208; 89;
        18; 169; 200; 196; 135; 130; 116; 188; 159; 86; 164; 100; 109; 198; 173; 186; 3; 64; 52; 217; 226; 250;
        124; 123; 5; 202; 38; 147; 118; 126; 255; 82; 85; 212; 207; 206; 59; 227; 47; 16; 58; 17; 182; 189;
        28; 42; 223; 183; 170; 213; 119; 248; 152; 2; 44; 154; 163; 70; 221; 153; 101; 155; 167; 43; 172; 9;
        129; 22; 39; 253; 19; 98; 108; 110; 79; 113; 224; 232; 178; 185; 112; 104; 218; 246; 97; 228; 251; 34;
        242; 193; 238; 210; 144; 12; 191; 179; 162; 241; 81; 51; 145; 235; 249; 14; 239; 107; 49; 192; 214; 31;
        181; 199; 106; 157; 184; 84; 204; 176; 115; 121; 50; 45; 127; 4; 150; 254; 138; 236; 205; 93; 222; 114;
        67; 29; 24; 72; 243; 141; 128; 195; 78; 66; 215; 61; 156; 180 |]

    let RandomSize = 256
    let Sqrt3 = 1.7320508075688772935
    let Sqrt5 = 2.2360679774997896964
    let mutable _random: int array = null
    let F2 = 0.5 * (Sqrt3 - 1.0)
    let G2 = (3.0 - Sqrt3) / 6.0
    let G22 = G2 * 2.0 - 1.0

    let F3 = 1.0 / 3.0
    let G3 = 1.0 / 6.0

    let F4 = (Sqrt5 - 1.0) / 4.0
    let G4 = (5.0 - Sqrt5) / 20.0
    let G42 = G4 * 2.0
    let G43 = G4 * 3.0
    let G44 = G4 * 4.0 - 1.0

    // let Grad3 = [|
    //     [| 1; 1; 0 |]; [| -1; 1; 0 |]; [| 1; -1; 0 |]
    //     [| -1; -1; 0 |]; [| 1; 0; 1 |]; [| -1; 0; 1 |]
    //     [| 1; 0; -1 |]; [| -1; 0; -1 |]; [| 0; 1; 1 |]
    //     [| 0; -1; 1 |]; [| 0; 1; -1 |]; [| 0; -1; -1 |]
    // |]
    let Grad3 = [|
        [| 1; 1; 0 |]; [| -1; 1; 0 |]; [| 1; -1; 0 |]
        [| -1; -1; 0 |]; [| 1; 0; 1 |]; [| -1; 0; 1 |]
        [| 1; 0; -1 |]; [| -1; 0; -1 |]; [| 0; 1; 1 |]
        [| 0; -1; 1 |]; [| 0; 1; -1 |]; [| 0; -1; -1 |]
    |]

    /// <summary>
    /// Unpack the given integer (int32) to an array of 4 bytes  in little endian format.
    /// If the length of the buffer is too smal, it wil be resized.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The output buffer.</returns>
    let unpackLittleUint32 value =
        let buffer: byte array = Array.zeroCreate 4
        buffer[0] <- byte (value &&& 0x00ff)
        buffer[1] <- byte ((value &&& 0xff00) >>> 8)
        buffer[2] <- byte ((value &&& 0x00ff0000) >>> 16)
        buffer[3] <- byte ((value &&& 0xff000000) >>> 24)
        buffer

    let randomize seed =
        _random <- Array.zeroCreate <| RandomSize * 2

        if seed <> 0 then
            // Shuffle the array using the given seed
            // Unpack the seed into 4 bytes then perform a bitwise XOR operation
            // with each byte
            let F = unpackLittleUint32 seed

            for i in 0 .. Source.Length - 1 do
                _random[i] <- Source[i] ^^^ int F[0]
                _random[i] <- _random[i] ^^^ int F[1]
                _random[i] <- _random[i] ^^^ int F[2]
                _random[i] <- _random[i] ^^^ int F[3]
                _random[i + RandomSize] <- _random[i]
        else
            for i in 0 .. RandomSize - 1 do
                _random[i] <- Source[i]
                _random[i + RandomSize] <- Source[i]

    let dot4 (g: int array) (x: float) (y: float) (z: float) (t: float) =
        float g[0] * x + float g[1] * y + float g[2] * z + float g[3] * t

    let dot3 (g: int array) (x: float) (y: float) (z: float) =
        float g[0] * x + float g[1] * y + float g[2] * z

    let dot2 (g: int array) (x: float) (y: float) = float g[0] * x + float g[1] * y

    let fastFloor (x: float) =
        if x >= 0.0 then int x else int (x - 1.0)

    do randomize seed
    new() = MyNoise(0)

    member this.Evaluate(point: Vector3) =
        let x = float point.X
        let y = float point.Y
        let z = float point.Z
        // Noise contributions from the four corners
        // Skew the input space to determine which simplex cell we're in
        let s = (x + y + z) * F3
        // for 3D
        let i = fastFloor <| x + s
        let j = fastFloor <| y + s
        let k = fastFloor <| z + s
        let t = float (i + j + k) * G3
        // The x,y,z distances from the cell origin
        let x0 = x - (float i - t)
        let y0 = y - (float j - t)
        let z0 = z - (float k - t)
        // For the 3D case, the simplex shape is a slightly irregular tetrahedron.
        // Determine which simplex we are in.
        // Offsets for second corner of simplex in (i,j,k)
        // i1, j1, k1
        // coords
        // i2, j2, k2
        // Offsets for third corner of simplex in (i,j,k) coords
        let i1, j1, k1, i2, j2, k2 =
            if x0 >= y0 then
                if y0 >= z0 then
                    // X Y Z order
                    1, 0, 0, 1, 1, 0
                elif x0 >= z0 then
                    // X Z Y order
                    1, 0, 0, 1, 0, 1
                else
                    // Z X Y order
                    0, 1, 0, 0, 1, 1
            // x0 < y0
            elif y0 < z0 then
                // Z Y X order
                0, 0, 1, 0, 1, 1
            elif x0 < z0 then
                // Y Z X order
                0, 1, 0, 0, 1, 1
            else
                // Y X Z order
                0, 1, 0, 1, 1, 0
        // A step of (1,0,0) in (i,j,k) means a step of (1-c,-c,-c) in (x,y,z),
        // a step of (0,1,0) in (i,j,k) means a step of (-c,1-c,-c) in (x,y,z),
        // and
        // a step of (0,0,1) in (i,j,k) means a step of (-c,-c,1-c) in (x,y,z),
        // where c = 1/6.

        // Offsets for second corner in (x,y,z) coords
        let x1 = y0 - float i1 + G3
        let y1 = z0 - float j1 + G3
        let z1 = x0 - float k1 + G3
        // Offsets for third corner in (x,y,z)
        let x2 = x0 - float i2 + F3
        let y2 = y0 - float j2 + F3
        let z2 = z0 - float k2 + F3
        // Offsets for last corner in (x,y,z)
        let x3 = x0 - 0.5
        let y3 = y0 - 0.5
        let z3 = z0 - 0.5
        // Work out the hashed gradient indices of the four simplex corners
        let ii = i &&& 0xff
        let jj = j &&& 0xff
        let kk = k &&& 0xff
        // Calculate the contribution from the four corners
        let t0 = 0.6 - x0 * x0 - y0 * y0 - z0 * z0

        let n0 =
            if t0 > 0 then
                let t0 = t0 * t0
                let gi0 = _random[ii + _random[jj + _random[kk]]] % 12
                t0 * t0 * dot3 Grad3[gi0] x0 y0 z0
            else
                0

        let t1 = 0.6 - x1 * x1 - y1 * y1 - z1 * z1

        let n1 =
            if t1 > 0 then
                let t1 = t1 * t1
                let gi1 = _random[ii + i1 + _random[jj + j1 + _random[kk + k1]]] % 12
                t1 * t1 * dot3 Grad3[gi1] x1 y1 z1
            else
                0

        let t2 = 0.6 - x2 * x2 - y2 * y2 - z2 * z2

        let n2 =
            if t2 > 0 then
                let t2 = t2 * t2
                let gi2 = _random[ii + i2 + _random[jj + j2 + _random[kk + k2]]] % 12
                t2 * t2 * dot3 Grad3[gi2] x2 y2 z2
            else
                0

        let t3 = 0.6 - x3 * x3 - y3 * y3 - z3 * z3

        let n3 =
            if t3 > 0 then
                let t3 = t3 * t3
                let gi3 = _random[ii + 1 + _random[jj + 1 + _random[kk + 1]]] % 12
                t3 * t3 * dot3 Grad3[gi3] x3 y3 z3
            else
                0
        // Add contributions from each corner to get the final noise value.
        // The result is scaled to stay just inside [-1,1]
        float32 <| (n0 + n1 + n2 + n3) * 32.0
