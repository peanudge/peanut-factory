REFERENCE MANUAL

MultiCam

MultiCam Storage Formats

© EURESYS s.a. 2025 - Doc. D406EN-MultiCam Storage Formats-6.19.4.4059 built on 2025-12-15

MultiCam MultiCam Storage Formats

This documentation is provided with MultiCam 6.19.4 (doc build 4059).
www.euresys.com

2

MultiCam MultiCam Storage Formats

Contents

1. Introduction

2. Monochrome Pixel Formats

2.1. 8-bit Monochrome
2.2. 10-bit Monochrome
2.3. 10-bit Monochrome lsb Packed
2.4. 10-bit Monochrome msb Packed
2.5. 12-bit Monochrome
2.6. 14-bit Monochrome
2.7. 16-bit Monochrome

3. Bayer CFA Pixel Formats

3.1. 8-bit Bayer CFA
3.2. 10-bit Bayer CFA
3.3. 12-bit Bayer CFA
3.4. 14-bit Bayer CFA
3.5. 16-bit Bayer CFA

4. RGB Color Pixel Formats

4.1. 5-5-5-bit BGR
4.2. 5-6-5-bit BGR
4.3. 8-bit BGR
4.4. 8-bit RGB
4.5. 8-bit BGRa
4.6. 8-bit RGBa
4.7. 10-bit BGR lsb Packed
4.8. 10-bit BGR msb Packed
4.9. 10-bit BGRa lsb Packed

5. RGB Color Planar Pixel Formats

5.1. 8-bit RGB Planar
5.2. 10-bit RGB Planar
5.3. 12-bit RGB Planar
5.4. 14-bit RGB Planar
5.5. 16-bit RGB Planar

6. YUV Color Pixel Formats

6.1. 8-bit YUV 4:1:1
6.2. 8-bit YUV 4:2:2
6.3. 8-bit YUV 4:4:4

7. YUV Color Planar Pixel Formats

7.1. 8-bit YUV 4:1:0 Planar
7.2. 8-bit YUV 4:1:1 Planar
7.3. 8-bit YUV 4:2:0 Planar
7.4. 8-bit YUV 4:2:2 Planar
7.5. 8-bit YUV 4:4:4 Planar

8. Raw Data Formats
8.1. 8-bit Raw Data
8.2. 10-bit Raw Data
8.3. 12-bit Raw Data
8.4. 14-bit Raw Data
8.5. 16-bit Raw Data

3

4

5
6
7
8
9
10
11
12

13
14
15
16
17
18

19
20
21
22
23
24
25
26
27
28

29
30
31
32
33
34

35
36
37
38

39
40
41
42
43
44

45
46
47
48
49
50

MultiCam MultiCam Storage Formats

1. Introduction

MultiCam frame grabbers store image pixel data into the user buffer according to a format
designated by the Channel parameter ColorFormat.

This document provides for each format:

● The MultiCam name, i.e. the ColorFormat value

● The PFNC name as specified in the Pixel Format Naming Convention of GenICam

● The storage type:

□ PACKED when all the components of a pixel are stored consecutively
□ PLANAR when each component of a pixel is stored separetely in different planes

● The memory layout describing how the first pixels of the first line(s) are stored in the user

buffer.

4

MultiCam MultiCam Storage Formats

2. Monochrome Pixel Formats

MultiCam Name

PFNC Name

Link

Y8

Y10

Y10P

N/A

Y12

Y14

Y16

Mono8

Mono10

"8-bit Monochrome" on page 6

"10-bit Monochrome" on page 7

Mono10p

"10-bit Monochrome lsb Packed" on page 8

Mono10pmsb

"10-bit Monochrome msb Packed" on page 9

Mono12

Mono14

Mono16

"12-bit Monochrome" on page 10

"14-bit Monochrome" on page 11

"16-bit Monochrome" on page 12

5

MultiCam MultiCam Storage Formats

2.1. 8-bit Monochrome

MultiCam Name

PFNC Name

Storage Type

Y8

Mono8

N/A

Storage
Requirement

1 Byte/pixel

Memory Layout

@

0

+3

+2

+1

+0

Pixel[3,0]

Pixel[2,0]

Pixel[1,0]

Pixel[0,0]

6

MultiCam MultiCam Storage Formats

2.2. 10-bit Monochrome

MultiCam Name

PFNC Name

Storage Type

Y10

Mono10

N/A

Storage
Requirement

2 Bytes/pixel

Memory Layout

@

0

+3

+2

+1

+0

0

0

0

0

0

0

Pixel[1,0]

0

0

0

0

0

0

Pixel[0,0]

7

MultiCam MultiCam Storage Formats

2.3. 10-bit Monochrome lsb Packed

MultiCam Name

PFNC Name

Storage Type

Storage
Requirement

Y10P

Mono10p

N/A

1.25 Bytes/pixel

Memory Layout

3
1

2
4

1
6

8

0

@

+3

+2

+1

+0

Pixel[2,0]

Pixel[1,0]

Pixel[0,0]

...
(1,
0)

...
(3,0)

...
(5:0)

0

4

8

1
2

1
6

Pixel[5,0]

Pixel[4,0]

Pixel[8,0]

Pixel[7,0]

Pixel[3,0]
(9:2)...

Pixel[6,0]
(9:4)...

Pixel[9,0]
(9:6)...

Pixel[12,0]
...(7:0)

Pixel[11,0]

Pixel[10,0]

Pixel[15,0]

Pixel[14,0]

Pixel[13,0]

....
(9:
8)

NOTE
A pixel boundary is aligned to a 32-bit word boundary every 16 pixels (20
bytes)

8

MultiCam MultiCam Storage Formats

2.4. 10-bit Monochrome msb Packed

MultiCam Name

PFNC Name

Storage Type

Storage
Requirement

N/A

Mono10pmsb

N/A

1.25 Bytes/pixel

Memory Layout

3
1

2
4

1
6

8

@

+0

+1

+2

+3

Pixel[0,0]

Pixel[1,0]

Pixel[2,0]

Pixel[3,0]
...(7:0)

Pixel[4,0]

Pixel[5,0]

Pixel[7,0]

Pixel[8,0]

Pixel[10,0]

Pixel[11,0]

0

4

8

1
2

1
6

...
(5:0)

...
(3,0)

...
(1,
0)

Pixel[13,0]

Pixel[14,0]

Pixel[15,0]

0

...
(9:
8)

Pixel[6,0]
(9:6)...

Pixel[9,0]
(9:4)...

Pixel[12,0]
(9:2)...

NOTE
A pixel boundary is aligned to a 32-bit word boundary every 16 pixels (20
bytes)

9

MultiCam MultiCam Storage Formats

2.5. 12-bit Monochrome

MultiCam Name

PFNC Name

Storage Type

Y12

Mono12

N/A

Storage
Requirement

2 Bytes/pixel

Memory Layout

@

0

+3

+2

+1

+0

0

0

0

0

Pixel[1,0]

0

0

0

0

Pixel[0,0]

10

MultiCam MultiCam Storage Formats

2.6. 14-bit Monochrome

MultiCam Name

PFNC Name

Storage Type

Y14

Mono14

N/A

Storage
Requirement

2 Bytes/pixel

Memory Layout

@

0

+3

+2

+1

+0

0

0

Pixel[1,0]

0

0

Pixel[0,0]

11

MultiCam MultiCam Storage Formats

2.7. 16-bit Monochrome

MultiCam Name

PFNC Name

Storage Type

Y16

Mono16

N/A

Storage
Requirement

2 Bytes/pixel

Memory Layout

@

0

+3

+2

+1

+0

Pixel[1,0]

Pixel[0,0]

12

MultiCam MultiCam Storage Formats

3. Bayer CFA Pixel Formats

MultiCam Name

PFNC Name

Link

BAYER8

BAYER10

BAYER12

BAYER14

BAYER16

BayerBG8
BayerGB8
BayerGR8
BayerRG8

BayerBG10
BayerGB10
BayerGR10
BayerRG10

BayerBG12
BayerGB12
BayerGR12
BayerRG12

BayerBG14
BayerGB14
BayerGR14
BayerRG14

BayerBG16
BayerGB16
BayerGR16
BayerRG16

"8-bit Bayer CFA" on page 14

"10-bit Bayer CFA" on page 15

"12-bit Bayer CFA" on page 16

"14-bit Bayer CFA" on page 17

"16-bit Bayer CFA" on page 18

13

MultiCam MultiCam Storage Formats

3.1. 8-bit Bayer CFA

MultiCam Name

PFNC Name

Color Filter Array

Bayer8

BayerBG8

BayerGB8

BayerGR8

BayerRG8

Memory Layout - BG CFA

Storage
Requirement

1 Byte/pixel

@

0

:

H

:

+3

+2

+1

+0

Pixel[3,0]:G

Pixel[2,0]: B

Pixel[1,0]: G

Pixel[0,0]: B

:

:

:

:

Pixel[3,1]: R

Pixel[2,1]: G

Pixel[1,1]: R

Pixel[0,1]: G

:

:

:

:

NOTE
H = buffer pitch (in bytes)

14

MultiCam MultiCam Storage Formats

3.2. 10-bit Bayer CFA

MultiCam Name

PFNC Name

Color Filter Array

Bayer10

BayerBG10

BayerGB10

BayerGR10

BayerRG10

Memory Layout - BG CFA

Storage
Requirement

2 Bytes/pixel

@

0

:

0

0

0

H 0

0

0

:

+3

0

:

0

:

+2

0

0

Pixel[1,0]: G

0

0

0

:

0

0

Pixel[1,1]: R

0

0

0

:

+1

0

:

0

:

+0

0

0

Pixel[0,0]: B

:

0

0

Pixel[0,1]: G

:

NOTE
H = buffer pitch (in bytes)

15

MultiCam MultiCam Storage Formats

3.3. 12-bit Bayer CFA

MultiCam Name

PFNC Name

Color Filter Array

Bayer12

BayerBG12

BayerGB12

BayerGR12

BayerRG12

Memory Layout - BG CFA

Storage
Requirement

2 Bytes/pixel

@

0

:

H

:

+3

+2

+1

+0

0

0

0

0

Pixel[1,0]: G

0

0

0

0

Pixel[0,0]: B

0

0

0

:

:

0

:

Pixel[1,1]: R

0

0

0

:

:

:

:

0

Pixel[0,1]: G

:

NOTE
H = buffer pitch (in bytes)

16

MultiCam MultiCam Storage Formats

3.4. 14-bit Bayer CFA

MultiCam Name

PFNC Name

Color Filter Array

Bayer14

BayerBG14

BayerGB14

BayerGR14

BayerRG14

Memory Layout - BG CFA

Storage
Requirement

2 Bytes/pixel

@

0

:

H

:

0

0

0

0

+3

:

:

+2

Pixel[1,0]: G

Pixel[1,1]: R

:

:

0

0

0

0

+1

:

:

Pixel[0,0]: B

Pixel[0,1]: G

+0

:

:

NOTE
H = buffer pitch (in bytes)

17

MultiCam MultiCam Storage Formats

3.5. 16-bit Bayer CFA

MultiCam Name

PFNC Name

Color Filter Array

Storage
Requirement

2 Bytes/pixel

Bayer16

BayerBG16

BayerGB16

BayerGR16

BayerRG16

Memory Layout - BG CFA

@

0

:

H

:

+3

:

:

Pixel[1,0]: G

Pixel[1,1]: R

+2

:

:

NOTE
H = buffer pitch (in bytes)

+1

:

:

Pixel[0,0]: B

Pixel[0,1]: G

+0

:

:

18

MultiCam MultiCam Storage Formats

4. RGB Color Pixel Formats

MultiCam Name

PFNC Name

Link

RGB15

RGB16

RGB24

N/A

RGB32

N/A

BGR555

BGR565

BGR8

RGB8

BGRa8

RGBa8

"5-5-5-bit BGR" on page 20

"5-6-5-bit BGR" on page 21

"8-bit BGR " on page 22

"8-bit RGB" on page 23

"8-bit BGRa" on page 24

"8-bit RGBa" on page 25

RGB30P

BGR10p

"10-bit BGR lsb Packed" on page 26

N/A

BGR10pmsb

"10-bit BGR msb Packed" on page 27

RGBI40P

BGRa10p

"10-bit BGRa lsb Packed" on page 28

19

MultiCam MultiCam Storage Formats

4.1. 5-5-5-bit BGR

MultiCam Name

PFNC Name

Storage Type

RGB15

BGR555

PACKED

Storage
Requirement

2 Bytes/pixel

Memory Layout

@

+3

+2

+1

+0

0

0

Pixel[1,0]:
R

Pixel[1,0]:
G

Pixel[1,0]:
B

0

Pixel[0,0]:
R

Pixel[0,0]:
G

Pixel[0,0]:
B

20

MultiCam MultiCam Storage Formats

4.2. 5-6-5-bit BGR

MultiCam Name

PFNC Name

Storage Type

RGB16

BGR565

PACKED

Storage
Requirement

2 Bytes/pixel

Memory Layout

@

+3

+2

+1

0

Pixel[1,0]: R

Pixel[1,0]: G

Pixel[1,0]: B

Pixel[0,0]: R

Pixel[0,0]: G

+0

Pixel[0,0]:
B

21

MultiCam MultiCam Storage Formats

4.3. 8-bit BGR

MultiCam Name

PFNC Name

Storage Type

RGB24

BGR8

PACKED

Storage
Requirement

3 Bytes/pixel

Memory Layout

@

0

4

8

+3

Pixel[1,0]: B

Pixel[2,0]: G

Pixel[3,0]: R

+2

Pixel[0,0]: R

Pixel[2,0]: B

Pixel[3,0]: G

+1

+0

Pixel[0,0]: G

Pixel[0,0]: B

Pixel[1,0]: R

Pixel[1,0]: G

Pixel[3,0]: B

Pixel[2,0]: R

22

MultiCam MultiCam Storage Formats

4.4. 8-bit RGB

MultiCam Name

PFNC Name

Storage Type

N/A

BGR8

PACKED

Storage
Requirement

3 Bytes/pixel

Memory Layout

@

0

4

8

+3

Pixel[1,0]: R

Pixel[2,0]: G

Pixel[3,0]: B

+2

Pixel[0,0]: B

Pixel[2,0]: R

Pixel[3,0]: G

+1

+0

Pixel[0,0]: G

Pixel[0,0]: R

Pixel[1,0]: B

Pixel[1,0]: G

Pixel[3,0]: R

Pixel[2,0]: B

23

MultiCam MultiCam Storage Formats

4.5. 8-bit BGRa

MultiCam Name

PFNC Name

Storage Type

RGB32

BGRa8

PACKED

Storage
Requirement

4 Bytes/pixel

Memory Layout

@

0

+3

+2

+1

+0

0

0

0

0

0

0

0

0

Pixel[0,0]: R

Pixel[0,0]: G

Pixel[0,0]: B

24

MultiCam MultiCam Storage Formats

4.6. 8-bit RGBa

MultiCam Name

PFNC Name

Storage Type

N/A

RGBa8

PACKED

Storage
Requirement

4 Bytes/pixel

Memory Layout

@

0

+3

+2

+1

+0

0

0

0

0

0

0

0

0

Pixel[0,0]: B

Pixel[0,0]: G

Pixel[0,0]: R

25

MultiCam MultiCam Storage Formats

4.7. 10-bit BGR lsb Packed

MultiCam Name

PFNC Name

Storage Type

Storage
Requirement

RGB30P

BGR10p

N/A

3.75 Bytes/pixel

Memory Layout

3
1

2
4

1
6

8

0

@

+3

+2

+1

+0

Pixel[0,0]R

Pixel[0,0]G

Pixel[0,0]B

...
(1,
0)

...
(3,0)

...
(5:0)

0

4

8

1
2

1
6

:

5
6

Pixel[1,0]R

Pixel[1,0]G

Pixel[2,0]R

Pixel[2,0]G

Pixel[1,0]B
(9:2)...

Pixel[2,0]B
(9:4)...

Pixel
[3,0]B
(9:6)...

Pixel[4,0]B
...(7:0)

Pixel[3,0]R

Pixel[3,0]G

Pixel[5,0]B

Pixel[4,0]R

Pixel[4,0]G

Pixel[15,0]R

Pixel[15,0]G

Pixel[15,0]B

NOTE
A pixel boundary is aligned to a 32-bit word boundary every 16 pixels (60
bytes)

...
(9:
8)

...
(9:
8)

26

0

4

8

1
2

1
6

:

5
6

MultiCam MultiCam Storage Formats

4.8. 10-bit BGR msb Packed

MultiCam Name

PFNC Name

Storage Type

Storage
Requirement

N/A

BGR10pmsb

N/A

1.25 Bytes/pixel

Memory Layout

3
1

2
4

1
6

8

@

+0

+1

+2

+3

Pixel[0,0]B

Pixel[0,0]G

Pixel[0,0]R

0

...
(9:
8)

Pixel[1,0]B
...(7:0)

...
(5:0)

...
(3,0)

Pixel[1,0]G

Pixel[1,0]R

Pixel[2,0]G

Pixel[2,0]R

Pixel[3,0]G

Pixel[3,0]R

Pixel
[2,0]B
(9:6)...

Pixel[3,0]B
(9:4)...

Pixel[4,0]B
(9:2)...

...
(1,
0)

...
(1,
0)

Pixel[4,0]G

Pixel[4,0]R

Pixel[5,0]B

Pixel[15,0]B

Pixel[15,0]G

Pixel[15,0]R

NOTE
A pixel boundary is aligned to a 32-bit word boundary every 16 pixels (60
bytes)

27

MultiCam MultiCam Storage Formats

4.9. 10-bit BGRa lsb Packed

MultiCam Name

PFNC Name

Storage Type

RGBI40P

BGRa10p

N/A

Storage
Requirement

5 Bytes/pixel

Memory Layout

3
1

2
4

1
6

8

0

@

+3

+2

+1

+0

0

4

8

1
2

1
6

...
(1,
0)

...
(3,0)

...
(5:0)

Pixel[0,0]R

Pixel[0,0]G

Pixel[0,0]B

Pixel[1,0]G

Pixel[1,0]B

Pixel[2,0]B

Pixel[1,0]a

Pixel[0,0]a
(9:2)...

Pixel[1,0]R
(9:4)...

Pixel[3,0]B
...(7:0)

Pixel[2,0]a

Pixel[2,0]R

Pixel[3,0]a

Pixel[3,0]R

Pixel[3,0]G

Pixel
[2,0]G
(9:6)...

...
(9:
8)

NOTE
A pixel boundary is aligned to a 32-bit word boundary every 4 pixels (20
bytes)

28

MultiCam MultiCam Storage Formats

5. RGB Color Planar Pixel Formats

MultiCam Name

PFNC Name

Link

RGB24PL

RGB30PL

RGB36PL

RGB42PL

RGB48PL

N/A

N/A

N/A

N/A

N/A

"8-bit RGB Planar" on page 30

"10-bit RGB Planar" on page 31

"12-bit RGB Planar" on page 32

"14-bit RGB Planar" on page 33

"16-bit RGB Planar" on page 34

29

MultiCam MultiCam Storage Formats

5.1. 8-bit RGB Planar

MultiCam Name

PFNC Name

Storage Type

RGB24PL

N/A

PLANAR

Storage
Requirement

3 Bytes/pixel

Memory Layout - Plane 0

@

0

+3

+2

+1

+0

Pixel[3,0]: R

Pixel[2,0]: R

Pixel[1,0]: R

Pixel[0,0]: R

Memory Layout - Plane 1

@

0

+3

+2

+1

+0

Pixel[3,0]: G

Pixel[2,0]: G

Pixel[1,0]: G

Pixel[0,0]: G

Memory Layout - Plane 2

@

0

+3

+2

+1

+0

Pixel[3,0]: B

Pixel[2,0]: B

Pixel[1,0]: B

Pixel[0,0]: B

30

MultiCam MultiCam Storage Formats

5.2. 10-bit RGB Planar

MultiCam Name

PFNC Name

Storage Type

RGB30PL

N/A

PLANAR

Storage
Requirement

6 Bytes/pixel

Memory Layout - Plane 0

@

0

+3

+2

+1

+0

0

0

0

0

0

0

Pixel[1,0]: R

0

0

0

0

0

0

Pixel[0,0]: R

Memory Layout - Plane 1

@

0

+3

+2

+1

+0

0

0

0

0

0

0

Pixel[1,0]: G

0

0

0

0

0

0

Pixel[0,0]: G

Memory Layout - Plane 2

@

0

+3

+2

+1

+0

0

0

0

0

0

0

Pixel[1,0]: B

0

0

0

0

0

0

Pixel[0,0]: B

31

MultiCam MultiCam Storage Formats

5.3. 12-bit RGB Planar

MultiCam Name

PFNC Name

Storage Type

RGB36PL

N/A

PLANAR

Storage
Requirement

6 Bytes/pixel

Memory Layout - Plane 0

@

0

+3

+2

+1

+0

0

0

0

0

Pixel[1,0]: R

0

0

0

0

Pixel[0,0]: R

Memory Layout - Plane 1

@

0

+3

+2

+1

+0

0

0

0

0

Pixel[1,0]: G

0

0

0

0

Pixel[0,0]: G

Memory Layout - Plane 2

@

0

+3

+2

+1

+0

0

0

0

0

Pixel[1,0]: B

0

0

0

0

Pixel[0,0]: B

32

MultiCam MultiCam Storage Formats

5.4. 14-bit RGB Planar

MultiCam Name

PFNC Name

Storage Type

RGB42PL

N/A

PLANAR

Storage
Requirement

6 Bytes/pixel

Memory Layout - Plane 0

@

0

+3

+2

+1

+0

0

0

Pixel[1,0]: R

0

0

Pixel[0,0]: R

Memory Layout - Plane 1

@

0

+3

+2

+1

+0

0

0

Pixel[1,0]: G

0

0

Pixel[0,0]: G

Memory Layout - Plane 2

@

0

+3

+2

+1

+0

0

0

Pixel[1,0]: B

0

0

Pixel[0,0]: B

33

MultiCam MultiCam Storage Formats

5.5. 16-bit RGB Planar

MultiCam Name

PFNC Name

Storage Type

RGB48PL

N/A

PLANAR

Storage
Requirement

6 Bytes/pixel

Memory Layout - Plane 0

@

0

+3

Pixel[1,0]: R

Memory Layout - Plane 1

@

0

+3

Pixel[1,0]: G

Memory Layout - Plane 2

@

0

+3

+2

+2

+2

+1

+1

+1

Pixel[0,0]: R

Pixel[0,0]: G

+0

+0

+0

Pixel[1,0]: B

Pixel[0,0]: B

34

MultiCam MultiCam Storage Formats

6. YUV Color Pixel Formats

MultiCam Name

PFNC Name

Link

YUV411
Y41P

YUV422
Y42P

YUV444
IYU2

YUV411_8_UYVYUYVYYYYY

"8-bit YUV 4:1:1" on page 36

YUV422_8_YUYV

"8-bit YUV 4:2:2" on page 37

YUV444_8_UYV

"8-bit YUV 4:4:4" on page 38

35

MultiCam MultiCam Storage Formats

6.1. 8-bit YUV 4:1:1

MultiCam Name

PFNC Name

Storage Type

Storage
Requirement

YUV411

Y41P

YUV411_8_
UYVYUYVYYYYY

PACKED

1.5 Bytes/pixel

Memory Layout

@

0

4

8

+3

+2

+1

+0

Pixel[1,0]: Y

Pixel[0,0]: V(Cr)

Pixel[0,0]: Y

Pixel[0,0]: U(Cb)

Pixel[3,0]: Y

Pixel[4,0]: V(Cr)

Pixel[2,0]: Y

Pixel[4,0]: U(Cb)

Pixel[7,0]: Y

Pixel[6,0]: Y

Pixel[5,0]: Y

Pixel[4,0]: Y

NOTE
Only 1 U(Cb) and 1 V(Cr) samples are captured every 4 pixels

36

MultiCam MultiCam Storage Formats

6.2. 8-bit YUV 4:2:2

MultiCam Name

PFNC Name

Storage Type

Storage
Requirement

YUV422

Y42P

Memory Layout

YUV422_8_YUYV

PACKED

2 Bytes/pixel

@

0

+3

+2

+1

+0

Pixel[0,0]: V(Cr)

Pixel[1,0]: Y

Pixel[0,0]: U(Cb)

Pixel[0,0]: Y

NOTE
Only 1 U(Cb) and 1 V(Cr) samples are captured every 2 pixels

37

MultiCam MultiCam Storage Formats

6.3. 8-bit YUV 4:4:4

MultiCam Name

PFNC Name

Storage Type

Storage
Requirement

YUV444

IYU2

Memory Layout

YUV444_8_UYV

PACKED

3 Bytes/pixel

@

0

4

8

+3

+2

+1

+0

Pixel[1,0]: U(Cb)

Pixel[0,0]: V(Cr)

Pixel[0,0]: Y

Pixel[0,0]: U(Cb)

Pixel[2,0]: Y

Pixel[2,0]: U(Cb)

Pixel[1,0]: V(Cr)

Pixel[1,0]: Y

Pixel[3,0]: V(Cr)

Pixel[3,0]: Y

Pixel[3,0]: U(Cb)

Pixel[2,0]: V(Cr)

38

MultiCam MultiCam Storage Formats

7. YUV Color Planar Pixel Formats

MultiCam Name

PFNC Name

Link

YUV411PL_Dec
YUV9
YVU9

YUV411PL
Y41B

YUV422PL_Dec
I420
IYUV
YV12

YUV422PL
Y42B

YUV410_8_Planar

"8-bit YUV 4:1:0 Planar" on page 40

YUV411_8_Planar

"8-bit YUV 4:1:1 Planar" on page 41

YUV420_8_Planar

"8-bit YUV 4:2:0 Planar" on page 42

YUV422_8_Planar

"8-bit YUV 4:2:2 Planar" on page 43

YUV444PL

YUV444_8_Planar

"8-bit YUV 4:4:4 Planar" on page 44

39

MultiCam MultiCam Storage Formats

7.1. 8-bit YUV 4:1:0 Planar

MultiCam Name

PFNC Name

Storage Type

Storage
Requirement

YUV411PL_Dec

YUV9

YVU9

YUV410_8_Planar

PLANAR

1.125 Bytes/pixel

Memory Layout - Plane 0

@

0

+3

+2

+1

+0

Pixel[3,0]: Y

Pixel[2,0]: Y

Pixel[1,0]: Y

Pixel[0,0]: Y

NOTE
H = buffer pitch (in bytes)

Memory Layout - Plane 1

@

0

:

H

:

+3

+2

+1

+0

Pixel[12,0]: U(Cb)

Pixel[8,0]: U(Cb)

Pixel[4,0]: U(Cb)

Pixel[0,0]: U(Cb)

:

:

:

:

Pixel[12,4]: U(Cb)

Pixel[8,4]: U(Cb)

Pixel[4,4]: U(Cb)

Pixel[0,4]: U(Cb)

:

:

:

:

NOTE
Only 1 U(Cb) sample is captured every 4 pixels in 1 line every 4 lines

Memory Layout - Plane 2

@

0

:

0

:

+3

+2

+1

+0

Pixel[12,0]: V(Cr)

Pixel[8,0]: V(Cr)

Pixel[4,0]: V(Cr)

Pixel[0,0]: V(Cr)

:

:

:

:

Pixel[12,4]: V(Cr)

Pixel[8,4]: V(Cr)

Pixel[4,4]: V(Cr)

Pixel[0,4]: V(Cr)

:

:

:

:

NOTE
Only 1 V(Cr) sample is captured every 4 pixels in 1 line every 4 lines

40

MultiCam MultiCam Storage Formats

7.2. 8-bit YUV 4:1:1 Planar

MultiCam Name

PFNC Name

Storage Type

Storage
Requirement

YUV411PL

Y41B

YUV411_8_Planar

PLANAR

1.5 Bytes/pixel

Memory Layout - Plane 0

@

0

4

8

12

+3

Pixel[3,0]: Y

Pixel[7,0]: Y

+2

Pixel[2,0]: Y

Pixel[6,0]: Y

Pixel[11,0]: Y

Pixel[10,0]: Y

+1

Pixel[1,0]: Y

Pixel[5,0]: Y

Pixel[9,0]: Y

+0

Pixel[0,0]: Y

Pixel[4,0]: Y

Pixel[8,0]: Y

Pixel[15,0]: Y

Pixel[14,0]: Y

Pixel[13,0]: Y

Pixel[12,0]: Y

Memory Layout - Plane 1

@

0

+3

+2

+1

+0

Pixel[12,0]: U(Cb)

Pixel[8,0]: U(Cb)

Pixel[4,0]: U(Cb)

Pixel[0,0]: U(Cb)

NOTE
Only 1 U(Cb) sample is captured every 4 pixels

Memory Layout - Plane 2

@

0

+3

+2

+1

+0

Pixel[12,0]: V(Cr)

Pixel[8,0]: V(Cr)

Pixel[4,0]: V(Cr)

Pixel[0,0]: V(Cr)

NOTE
Only 1 V(Cr) sample is captured every 4 pixels

41

MultiCam MultiCam Storage Formats

7.3. 8-bit YUV 4:2:0 Planar

MultiCam Name

PFNC Name

Storage Type

Storage
Requirement

YUV422PL_Dec

I420

IYUV

YV12

YUV420_8_Planar

PLANAR

1.5 Bytes/pixel

Memory Layout - Plane 0

@

0

+3

+2

+1

+0

Pixel[3,0]: Y

Pixel[2,0]: Y

Pixel[1,0]: Y

Pixel[0,0]: Y

Memory Layout - Plane 1

@

0

:

2H

:

+3

+2

+1

+0

Pixel[6,0]: U(Cb)

Pixel[4,0]: U(Cb)

Pixel[2,0]: U(Cb)

Pixel[0,0]: U(Cb)

:

:

:

:

Pixel[6,2]: U(Cb)

Pixel[4,2]: U(Cb)

Pixel[2,2]: U(Cb)

Pixel[0,2]: U(Cb)

:

:

:

:

NOTE
Only 1 U(Cb) sample is captured every 2 pixels in 1 line every 2 lines

Memory Layout - Plane 2

@

0

:

H

:

+3

+2

+1

+0

Pixel[6,0]: V(Cr)

Pixel[4,0]: V(Cr)

Pixel[2,0]: V(Cr)

Pixel[0,0]: V(Cr)

:

:

:

:

Pixel[6,2]: V(Cr)

Pixel[4,2]: V(Cr)

Pixel[2,2]: V(Cr)

Pixel[0,2]: V(Cr)

:

:

:

:

NOTE
Only 1 V(Cr) sample is captured every 2 pixels in 1 line every 2 lines

42

MultiCam MultiCam Storage Formats

7.4. 8-bit YUV 4:2:2 Planar

MultiCam Name

PFNC Name

Storage Type

Storage
Requirement

YUV422PL

Y42B

YUV422_8_Planar

PLANAR

2 Bytes/pixel

Memory Layout - Plane 0

@

0

4

+3

Pixel[3,0]: Y

Pixel[7,0]: Y

+2

Pixel[2,0]: Y

Pixel[6,0]: Y

+1

Pixel[1,0]: Y

Pixel[5,0]: Y

+0

Pixel[0,0]: Y

Pixel[4,0]: Y

Memory Layout - Plane 1

@

0

+3

+2

+1

+0

Pixel[6,0]: U(Cb)

Pixel[4,0]: U(Cb)

Pixel[2,0]: U(Cb)

Pixel[0,0]: U(Cb)

NOTE
Only 1 U(Cb) sample is captured every 2 pixels

Memory Layout - Plane 2

@

0

+3

+2

+1

+0

Pixel[6,0]: V(Cr)

Pixel[4,0]: V(Cr)

Pixel[2,0]: V(Cr)

Pixel[0,0]: V(Cr)

NOTE
Only 1 V(Cr) sample is captured every 2 pixels

43

MultiCam MultiCam Storage Formats

7.5. 8-bit YUV 4:4:4 Planar

MultiCam Name

PFNC Name

Storage Type

YUV444PL

YUV444_8_Planar

PLANAR

Storage
Requirement

3 Bytes/pixel

Memory Layout - Plane 0

@

0

+3

+2

+1

+0

Pixel[3,0]: Y

Pixel[2,0]: Y

Pixel[1,0]: Y

Pixel[0,0]: Y

Memory Layout - Plane 1

@

0

+3

+2

+1

+0

Pixel[3,0]: U(Cb)

Pixel[2,0]: U(Cb)

Pixel[1,0]: U(Cb)

Pixel[0,0]: U(Cb)

Memory Layout - Plane 2

@

0

+3

+2

+1

+0

Pixel[3,0]: V(Cr)

Pixel[2,0]: V(Cr)

Pixel[1,0]: V(Cr)

Pixel[0,0]: V(Cr)

44

MultiCam MultiCam Storage Formats

8. Raw Data Formats

MultiCam Name

PFNC Name

Link

RAW8

RAW10

RAW12

RAW14

RAW16

Raw8

Raw10

Raw12

Raw14

Raw16

"8-bit Raw Data" on page 46

"10-bit Raw Data" on page 47

"12-bit Raw Data" on page 48

"14-bit Raw Data" on page 49

"16-bit Raw Data" on page 50

8.1. 8-bit Raw Data

8.2. 10-bit Raw Data

8.3. 12-bit Raw Data

8.4. 14-bit Raw Data

8.5. 16-bit Raw Data

46

47

48

49

50

45

MultiCam MultiCam Storage Formats

8.1. 8-bit Raw Data

MultiCam Name

PFNC Name

Storage Type

RAW8

Raw8

N/A

Storage
Requirement

1 Byte/pixel

Memory Layout

@

0

+3

+2

+1

+0

Pixel[3,0]

Pixel[2,0]

Pixel[1,0]

Pixel[0,0]

46

MultiCam MultiCam Storage Formats

8.2. 10-bit Raw Data

MultiCam Name

PFNC Name

Storage Type

RAW10

Raw10

N/A

Storage
Requirement

2 Bytes/pixel

Memory Layout

@

0

+3

+2

+1

+0

0

0

0

0

0

0

Pixel[1,0]

0

0

0

0

0

0

Pixel[0,0]

47

MultiCam MultiCam Storage Formats

8.3. 12-bit Raw Data

MultiCam Name

PFNC Name

Storage Type

RAW12

Raw12

N/A

Storage
Requirement

2 Bytes/pixel

Memory Layout

@

0

+3

+2

+1

+0

0

0

0

0

Pixel[1,0]

0

0

0

0

Pixel[0,0]

48

MultiCam MultiCam Storage Formats

8.4. 14-bit Raw Data

MultiCam Name

PFNC Name

Storage Type

RAW14

Raw14

N/A

Storage
Requirement

2 Bytes/pixel

Memory Layout

@

0

+3

+2

+1

+0

0

0

Pixel[1,0]

0

0

Pixel[0,0]

49

MultiCam MultiCam Storage Formats

8.5. 16-bit Raw Data

MultiCam Name

PFNC Name

Storage Type

RAW16

Raw16

N/A

Storage
Requirement

2 Bytes/pixel

Memory Layout

@

0

+3

+2

+1

+0

Pixel[1,0]

Pixel[0,0]

50


