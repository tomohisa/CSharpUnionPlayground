using UnionTest;
using Xunit;

namespace UnionTest.Tests;

public class Union100Tests
{
    [Fact]
    public void Case1_AssignAndMatch()
    {
        Union100 u = new Case1(1);
        var result = u.Value switch
        {
            Case1 c => c.Value,
            _ => -1
        };
        Assert.Equal(1, result);
    }

    [Fact]
    public void Case50_AssignAndMatch()
    {
        Union100 u = new Case50(50);
        var result = u.Value switch
        {
            Case50 c => c.Value,
            _ => -1
        };
        Assert.Equal(50, result);
    }

    [Fact]
    public void Case100_AssignAndMatch()
    {
        Union100 u = new Case100(100);
        var result = u.Value switch
        {
            Case100 c => c.Value,
            _ => -1
        };
        Assert.Equal(100, result);
    }

    [Fact]
    public void AllCases_AssignAndVerify()
    {
        for (int i = 1; i <= 100; i++)
        {
            Union100 u = i switch
            {
                1 => new Case1(i),
                2 => new Case2(i),
                3 => new Case3(i),
                4 => new Case4(i),
                5 => new Case5(i),
                6 => new Case6(i),
                7 => new Case7(i),
                8 => new Case8(i),
                9 => new Case9(i),
                10 => new Case10(i),
                11 => new Case11(i),
                12 => new Case12(i),
                13 => new Case13(i),
                14 => new Case14(i),
                15 => new Case15(i),
                16 => new Case16(i),
                17 => new Case17(i),
                18 => new Case18(i),
                19 => new Case19(i),
                20 => new Case20(i),
                21 => new Case21(i),
                22 => new Case22(i),
                23 => new Case23(i),
                24 => new Case24(i),
                25 => new Case25(i),
                26 => new Case26(i),
                27 => new Case27(i),
                28 => new Case28(i),
                29 => new Case29(i),
                30 => new Case30(i),
                31 => new Case31(i),
                32 => new Case32(i),
                33 => new Case33(i),
                34 => new Case34(i),
                35 => new Case35(i),
                36 => new Case36(i),
                37 => new Case37(i),
                38 => new Case38(i),
                39 => new Case39(i),
                40 => new Case40(i),
                41 => new Case41(i),
                42 => new Case42(i),
                43 => new Case43(i),
                44 => new Case44(i),
                45 => new Case45(i),
                46 => new Case46(i),
                47 => new Case47(i),
                48 => new Case48(i),
                49 => new Case49(i),
                50 => new Case50(i),
                51 => new Case51(i),
                52 => new Case52(i),
                53 => new Case53(i),
                54 => new Case54(i),
                55 => new Case55(i),
                56 => new Case56(i),
                57 => new Case57(i),
                58 => new Case58(i),
                59 => new Case59(i),
                60 => new Case60(i),
                61 => new Case61(i),
                62 => new Case62(i),
                63 => new Case63(i),
                64 => new Case64(i),
                65 => new Case65(i),
                66 => new Case66(i),
                67 => new Case67(i),
                68 => new Case68(i),
                69 => new Case69(i),
                70 => new Case70(i),
                71 => new Case71(i),
                72 => new Case72(i),
                73 => new Case73(i),
                74 => new Case74(i),
                75 => new Case75(i),
                76 => new Case76(i),
                77 => new Case77(i),
                78 => new Case78(i),
                79 => new Case79(i),
                80 => new Case80(i),
                81 => new Case81(i),
                82 => new Case82(i),
                83 => new Case83(i),
                84 => new Case84(i),
                85 => new Case85(i),
                86 => new Case86(i),
                87 => new Case87(i),
                88 => new Case88(i),
                89 => new Case89(i),
                90 => new Case90(i),
                91 => new Case91(i),
                92 => new Case92(i),
                93 => new Case93(i),
                94 => new Case94(i),
                95 => new Case95(i),
                96 => new Case96(i),
                97 => new Case97(i),
                98 => new Case98(i),
                99 => new Case99(i),
                100 => new Case100(i),
                _ => throw new InvalidOperationException()
            };

            var val = u.Value switch
            {
                Case1 c => c.Value,
                Case2 c => c.Value,
                Case3 c => c.Value,
                Case4 c => c.Value,
                Case5 c => c.Value,
                Case6 c => c.Value,
                Case7 c => c.Value,
                Case8 c => c.Value,
                Case9 c => c.Value,
                Case10 c => c.Value,
                Case11 c => c.Value,
                Case12 c => c.Value,
                Case13 c => c.Value,
                Case14 c => c.Value,
                Case15 c => c.Value,
                Case16 c => c.Value,
                Case17 c => c.Value,
                Case18 c => c.Value,
                Case19 c => c.Value,
                Case20 c => c.Value,
                Case21 c => c.Value,
                Case22 c => c.Value,
                Case23 c => c.Value,
                Case24 c => c.Value,
                Case25 c => c.Value,
                Case26 c => c.Value,
                Case27 c => c.Value,
                Case28 c => c.Value,
                Case29 c => c.Value,
                Case30 c => c.Value,
                Case31 c => c.Value,
                Case32 c => c.Value,
                Case33 c => c.Value,
                Case34 c => c.Value,
                Case35 c => c.Value,
                Case36 c => c.Value,
                Case37 c => c.Value,
                Case38 c => c.Value,
                Case39 c => c.Value,
                Case40 c => c.Value,
                Case41 c => c.Value,
                Case42 c => c.Value,
                Case43 c => c.Value,
                Case44 c => c.Value,
                Case45 c => c.Value,
                Case46 c => c.Value,
                Case47 c => c.Value,
                Case48 c => c.Value,
                Case49 c => c.Value,
                Case50 c => c.Value,
                Case51 c => c.Value,
                Case52 c => c.Value,
                Case53 c => c.Value,
                Case54 c => c.Value,
                Case55 c => c.Value,
                Case56 c => c.Value,
                Case57 c => c.Value,
                Case58 c => c.Value,
                Case59 c => c.Value,
                Case60 c => c.Value,
                Case61 c => c.Value,
                Case62 c => c.Value,
                Case63 c => c.Value,
                Case64 c => c.Value,
                Case65 c => c.Value,
                Case66 c => c.Value,
                Case67 c => c.Value,
                Case68 c => c.Value,
                Case69 c => c.Value,
                Case70 c => c.Value,
                Case71 c => c.Value,
                Case72 c => c.Value,
                Case73 c => c.Value,
                Case74 c => c.Value,
                Case75 c => c.Value,
                Case76 c => c.Value,
                Case77 c => c.Value,
                Case78 c => c.Value,
                Case79 c => c.Value,
                Case80 c => c.Value,
                Case81 c => c.Value,
                Case82 c => c.Value,
                Case83 c => c.Value,
                Case84 c => c.Value,
                Case85 c => c.Value,
                Case86 c => c.Value,
                Case87 c => c.Value,
                Case88 c => c.Value,
                Case89 c => c.Value,
                Case90 c => c.Value,
                Case91 c => c.Value,
                Case92 c => c.Value,
                Case93 c => c.Value,
                Case94 c => c.Value,
                Case95 c => c.Value,
                Case96 c => c.Value,
                Case97 c => c.Value,
                Case98 c => c.Value,
                Case99 c => c.Value,
                Case100 c => c.Value,
                null => -1,
                _ => -1
            };

            Assert.Equal(i, val);
        }
    }
}
