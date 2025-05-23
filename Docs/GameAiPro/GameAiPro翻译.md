https://www.gameaipro.com/

# 第 I 部分 通用智慧（General Wisdom）

# 3 游戏 AI 的高级随机性技术：高斯随机性、滤波随机性和 Perlin 噪声

*Steve Rabin, Jay Goldblatt, and Fernando Silva*

## 3.1 简介

游戏程序员与 rand() 函数有着特殊的关系。我们依靠它来改变我们的游戏，用它来保持我们的游戏玩法新鲜，并防止我们的NPC变得可预测。无论是决策、游戏事件还是动画选择，我们最不希望看到的是重复的角色或可预测的游戏玩法；因此，随机性已成为一种必不可少的工具。

然而，随机性是一种变化无常的野兽，人类尤其不善于理解或推理它。这使得人们很容易误用或误解随机性实际上提供了什么。本章将介绍我们信任的老朋友 rand() 根本无法提供的三种高级技术。

第一种技术涉及丢弃均匀随机性，并接受高斯随机性，以适应代理特征和行为的变化。无论是单位的速度、敌人的反应时间还是枪支的瞄准，在现实生活中，这些生物和物理现象都显示出正态（高斯）分布，而不是均匀分布。一旦理解了差异，你会发现高斯随机性在你制作的游戏中有几十种用途。

第二种技术是操纵随机性，使玩家在短时间内看起来更随机。众所周知，在观察小规模的孤立运行时，随机性看起来并不是随机的，所以我们的目标是用过滤后的随机性来解决这个问题。

最后一种技术是使用一种特殊类型的随机性，这种随机性不是均匀的或高斯的，而是产生一种漫步特征，其中连续的随机数彼此相关。Perlin 噪波通常用于图形中，可用于行为随时间随机变化的情况。无论是运动、准确性、愤怒、注意力，还是仅仅处于凹槽中，都有几十种行为特征可以使用一维 Perlin 噪声随时间变化。

对于这三种技术，本书的网站上都有演示和 C++ 库(http://www.gameaipro.com)你可以直接投入你的游戏。

## 3.2 技术 1：高斯随机性

正态分布（也称为高斯分布或钟形曲线）无处不在，隐藏在日常生活的统计数据中。我们在树的高度、建筑物的高度和人的高度上看到了这些分布。我们在购物中心漫步的购物者速度、马拉松比赛的跑步者速度和高速公路上的汽车速度上看到了这些分布。在任何有大量生物或事物的地方，我们都有这些生物或事物呈现正态分布的特征。

这些分布中存在随机性，但它们不是均匀随机的。例如，一个人长到 6 英尺高的机会与他长到5英尺高或7英尺高的最终高度的机会不同。如果机会相同，那么分布将是均匀随机的。相反，我们看到一个正态分布，男性的身高集中在 5 英尺 10 英寸左右，并在任何方向上以钟形曲线的形式逐渐下降。事实上，几乎每个身体和心理特征都有某种平均值，个体与平均值的差异呈正态分布。无论是身高、反应时间、视力还是智力，这些特征在特定人群中都会遵循正态分布。

生活中一些随机的事情确实显示出均匀的分布，比如生男孩或女孩的机会。然而，生命中的绝大多数分布比均匀分布更接近正态分布。但是为什么呢？

答案很简单，可以用中心极限定理来解释。基本上，当许多随机变量加在一起时，得到的总和将遵循正态分布。当你掷三个六面骰子时，可以看到这一点。虽然一个骰子落在任何一张脸上的几率是一致的，但掷三个骰子及其总和等于最大值 18 的几率与其他结果并不一致。例如，三个骰子加起来为 18 的几率为 0.5%，而三个骰子相加为 10 的几率为 12.5%。图 3.1 显示，掷三个骰子的总和实际上遵循正态分布。

既然我们已经证明了随机变量的添加会导致正态分布，那么问题仍然存在：为什么生活中的大多数分布都遵循正态分布？答案是，宇宙中几乎所有的东西都有不止一个影响因素，而这些影响因素都有随机的方面。

图 3.1
滚动三个六面骰子的概率之和将遵循正态分布，即使任何单个骰子的结果都具有均匀分布。这是由于中心极限定理。

例如，让我们以森林中成熟树高度的分布为例。是什么决定了一棵树能长多高？成熟树的高度受到其基因、降水、土壤质量、空气质量、日照量、温度以及昆虫或真菌暴露的影响。对于整个森林来说，每棵树都会经历每种品质的不同方面，这取决于树的位置（例如，在山坡上还是在山谷里）。即使是相邻的两棵树，其影响因素也会有细微的差异。从本质上讲，树的最终高度是每个单独因素影响的总和。换句话说，影响树木高度的品质的影响是累加的，因此它们导致所有树木的高度呈正态分布。正如你所想象的，有可能为生物系统或物理系统的几乎任何其他属性构建类似的论点。正态分布无处不在。

### 3.2.1 生成高斯随机性

既然我们已经展示了正态分布在现实生活中的普遍性，那么将它们包含在我们的游戏中是有意义的。考虑到这一点，你可能会想知道高斯随机性是如何产生的。前面的骰子例子给了我们一个线索。如果我们取三个均匀随机数（例如由 rand() 生成）并将它们加在一起，我们就可以生成具有正态分布的随机数。

更精确地说，中心极限定理指出，在 [-1, 1] 范围内添加均匀随机数将接近均值为零、标准差为 √(K/3) 的正态分布，其中 K 是求和的数字数量。如果我们选择 K 为 3，那么标准偏差等于 1，我们将得到接近标准正态分布。清单 3.1 中的代码显示了创建高斯随机性是多么容易。

清单 3.1 中的代码足以在游戏中生成高斯随机性。然而，有一些隐藏的细微差别需要解释。一个真正的正态分布将产生远远超出清单 3.1 提供的 [-3,3] 范围的值。虽然某些金融或医疗模拟可能需要一个准确的正态分布（有百万分之一的概率为 -4），但游戏不需要，因此最好保证生成的值在 [-3,3] 范围内。

Listing 3.1 高斯随机性可以通过将三个均匀随机数相加来生成。在这个例子中，均匀随机数是用 XOR 移位伪随机数生成器创建的。该函数返回 [-3.0, 3.0] 范围内的数字，其中 66.7% 落在一个标准偏差内，95.8% 落在两个标准偏差之内，100% 落在三个标准偏差以内。

```c++
unsigned long seed = 61829450;
double GaussianRand()
{
    double sum = 0;
    for (int i = 0; i < 3; i++)
    {
         unsigned long holdseed = seed;
         seed ^ = seed << 13;
         seed ^ = seed >> 17;
         seed ^ = seed << 5;
         long r = (Int64)(holdseed + seed);
         sum + = (double)r * (1.0/0x7FFFFFFFFFFFFFFF);
    }
    return sum; //returns [-3.0, 3.0] at (66.7%, 95.8%, 100%)
}
```

### 3.2.2 高斯随机性的应用

高斯随机性在人工智能中有很多用途[Mark 09]，但在游戏中非常明显的一个用途是投射物的瞄准[Rabin 08]。虽然许多游戏可能根本不会干扰投射物或使用均匀随机性来干扰它们，但正确的方法是应用高斯随机性。

图 3.2 显示了均匀和高斯子弹在目标上的传播（根据本书网站上的示例演示生成）。很明显，右边的高斯模型看起来更真实，但它是如何生成的呢？诀窍是使用极坐标以及均匀随机性和高斯随机性的混合。首先，生成一个从 0 到 360° 的随机均匀角度。该值用作极坐标，以确定围绕目标中心的角度。该值保持一致很重要，因为任何角度都应该有相等的机会。其次，生成随机高斯数以确定与目标中心的距离。通过结合随机均匀角度和距中心的随机高斯距离，您可以重建非常逼真的子弹散布。

高斯随机性在游戏中的其他应用包括 NPC 在人群中应该变化的任何方面。这些可能包括：

- 平均或最大速度
- 平均或最大加速度
- 尺寸、宽度、高度或质量
- 视觉或物理反应时间
- 射击或重新装弹率
- 治疗或特殊能力的刷新率或冷却率
- 错过或击中关键球的几率

需要考虑的一件事是，你是想在人群中的成员之间改变一个特征，还是每次使用时都改变这个特征，或者两者兼而有之。例如，一名士兵的固有射速可能比全体士兵的平均射速慢（在部队创建时使用高斯随机性确定），但该部队的任何一次射击都可能在部队固有射速的正态分布范围内变化（有时部队的射击速度会快一点，有时会慢一点）。

现在想象一个由 30 个单位组成的小组向敌人开火。如果每个单位都有一个固有的射速（正态分布），并且每个单位都在这个射速附近变化（也正态分布的），那么该团体的应急行为应该是非常自然的，没有单位会相互锁定射击。

图 3.2
目标上子弹散布的均匀分布和高斯分布。请注意，目标中的每个环代表一个标准偏差，66.7% 的高斯子弹击中最内环，95.8% 的子弹击中两个最内环。这个演示可以在书的网站上找到。

## 3.3 技术2：过滤随机性

这么说吧：随机性太随机了（在游戏中有很多用途）。起初，这似乎是一个荒谬的说法，但有大量证据表明，人类并不认为小范围的随机性是随机的[Diener等人，85，Wagenaar等人，91]。然而，这提出了一个有趣的后续问题：如果人类不认为小范围的随机性看起来是随机的，那么他们实际上是怎么想的？也许他们认为创建序列的东西要么被破坏、操纵，要么被欺骗——所有这些都是游戏或人工智能的可怕品质。

### 3.3.1 小规模的随机运行看起来并不随机

现在我们有了令人愤慨的声明，让我们支持它。首先，让我们确定小范围的随机性看起来并不随机。拿一张废纸，开始以随机顺序写下 0 和 1，每个数字的概率为 50%——这样做，直到你有一个 100 个数字的列表。去吧，真的试试这个。不，真的，这会让你成为一个更好的人。我们等着…。

现在，为了使这真正公平，拿出一枚硬币并开始抛硬币，将正面和反面的顺序记录为 0 和 1。将其翻转 100 次，制作一个与你用头脑创建的列表相当的列表。我们再次等待。如果你想获得丰厚的回报，你必须这样做…。

现在，我们还可以将您创建的两个列表与伪随机数生成器（PRNG）（如 rand()）创建的列表进行比较，其中 0 或 1 的概率为 50%。以下是随机生成的 100 次硬币翻转序列。

01101100001100001010000001001011110011100111000110
10101011011111101001011110011111101011111101000011

注意到你手工生成的列表、抛硬币列表和 PRNG 生成的列表之间有什么不同吗？与手动生成的列表相比，抛硬币和 PRNG 列表很可能更“块状”，包含更多长距离的 0 或 1。大多数人没有意识到的是，真正的随机性几乎总是包含这些长期运行，这些运行是非常典型的，也是意料之中的。然而，大多数人根本不相信公平的硬币或真正的随机性会产生长时间的正面或反面。事实上，在你自己真正抛硬币并看到它发生之前，很难将这一课内化（这就是我们希望你真正去做的原因）。

那么，这如何适用于游戏呢？许多游戏都包括均匀分布的随机数决定对玩家产生积极或消极影响的情况。如果玩家对某些赔率有预期，而游戏在短期内似乎对这些赔率产生了负面影响（尤其是结果对玩家造成了伤害），那么玩家就会认为游戏被打破或作弊[Meier 10]。记住，我们现在已经进入了心理学领域，暂时离开了数学[Mark 09]。如果玩家认为游戏是作弊，那么无论实际发生了什么，游戏实际上都是作弊；当涉及到玩家对游戏的享受时，感知比现实重要得多。

例如，想象一下，游戏设计师决定敌人应该在 10% 的时间里进行暴击。不幸的是，在 30 小时的游戏过程中经过多次战斗后，玩家连续三次被击中并受到严重打击的概率很高！游戏设计师是否曾希望这种情况发生？不！但这就是随机数的现实。在许多情况下，随机性对于游戏设计师真正想要的东西来说太随机了。

### 3.3.2 随机性看起来并不随机：解决方案

如何让随机数对人类来说看起来更随机？真实但无益的答案是以一种非常特殊的方式使数字稍微不那么随机。我们要做的是交换一点点随机性完整性，以实现一个对人类来说更随机的序列。

该策略非常简单：在生成随机数字序列时，如果下一个数字会损害随机性的外观，则假装你从未见过它并生成一个新的数字[Rabin 04]。事情就是这么简单。尽管如此，由于我们在这里处理的是心理学，因此这一策略的实际实施是主观的。此外，如果数字的消除过于热心，可能会损害随机性的完整性，因此必须仔细考虑实际实施。

### 3.3.3 识别异常

记住核心策略，第一个任务是确定使序列看起来不那么随机的事情。事实证明，实际上只有两个主要原因：

1. 该序列具有突出的图案，如硬币翻转序列 11001100 或 111000。
2. 该序列具有与硬币翻转序列 01011111110 中相同编号的长串。

我们可以将这两种原因归类为对人类来说看起来非随机（奇怪或不寻常）的“异常”。然后，目标是编写某种规则来识别这些异常。一旦我们有了规则，我们就可以抛出触发规则的最后一个数字（完成违规模式）。作为实现细节，我们的代码必须跟踪每个决策的最后 10 到 20 个生成的数字，以便我们的规则进行检查。我们将在后面的部分中对此进行更多探讨。

那么，规则是什么样子的呢？这取决于生成的随机性类型。抛硬币的规则将不同于随机范围或高斯随机性的规则。不幸的是，没有简单的方法可以解决这个问题，因为我们处理的是潜在的人类情感。

#### 3.3.3.1 过滤二进制随机性

如果希望有 50% 的机会，比如抛硬币，那么以下规则将以一种对人类来说更随机的方式过滤随机性。请注意，这是一个有序的规则列表，每次生成新的随机数时都要检查。此外，每个新生成的数字只允许触发一个规则。

1. 如果最新值将产生 4 或更多，则有 75% 的机会翻转最新值。这并不意味着 4 次或更多的跑步是不可能的，但越来越不可能（4 次跑步发生的概率从 1/8 到 1/128）。可以完全禁止特定长度的跑步，但这将对随机性的完整性产生更负面的影响。
2. 如果最新值导致四个值的重复模式，如 11001100，则翻转最后一个值（使序列变为 11001101）。
3. 如果最新值导致 111000 或 000111的重复模式，则翻转最后一个值。

从本文前面的二进制生成的随机序列来看，这是过滤前后的结果：

过滤前：

01101100001100001010000001001011110011100111000110
10101011011111101001011110011111101011111101000011

过滤后（带下划线的数字与原始序列切换）：

01101100011000101010001001001011110011100111001110
10101110011101101001011100111001101011101101000110

#### 3.3.3.2 过滤整数范围

与二进制随机性类似，可以构建规则来过滤出现在数字范围内的异常。以下是可以实施的相当激进的规则列表。对于这组规则，任何违反规则的行为都会导致重新轮询值，然后再次根据规则进行验证：

1. 重复数字，如“7，7”或“3，3”
2. 用一位数字分隔的重复数字，如“8，3，8”或“6，2，6”
3. 上升或下降的 4 的计数序列，如“3，4，5，6”
4. 最后 N 个值中某个范围的顶部或底部值太多，如“6、8、7、9、8、6、9”
5. 出现在最后 10 个值中的两个数字的模式，如“5,7,3,1,5,7”
6. 最后 10 个值中的特定数字太多，如“9、4、5、9、7、8、9、0、2、9”。

过滤前：

22312552222577750677564061448482102435500989388459
59607889964957780753281574605482138446235103745368

过滤后（突出显示的数字会被抛出，因为它们违反了规则）：

22312552222577750677564061448482102435500989388459
59607889964957780753281574605482138446235103745368

#### 3.3.3.3 滤波浮点范围

要过滤 [0,1] 范围内的浮点，我们必须设计规则，以避免类似数字的聚集，并避免增加或减少运行次数。如果违反了这些规则中的任何一条，我们只需抛出该值并要求一个必须通过所有规则的新随机数。

1. 如果两个连续数字的差值小于 0.02，如 0.875 和 0.856，则重新滚动。
2. 如果连续三个数字的差值小于 0.1，如 0.345、0.421 和 0.387，则重新滚动。
3. 如果有 5 个值的增加或减少，如 0.342、0.572、0.619、0.783 和 0.868，则重新滚动。
4. 如果最后 N 个值内范围顶部或底部的数字太多，如 0.325、0.198、0.056、0.432 和 0.216，请重新滚动。

#### 3.3.3.4 滤波高斯范围

由于高斯数与浮点数非常相似，因此适用相同的规则。但是，您可以引入以下规则来避免高斯数特有的特定异常。

5. 如果有四个连续的数字都高于或低于零，则重新滚动。
6. 如果有四个连续的数字位于第二个或第三个偏差范围内，则重新滚动。
7. 如果有两个连续的数字位于第三个偏差范围内，则重新滚动。

#### 3.3.3.5 随机完整性

最后四节中概述的规则是任意的，可以或多或少地改变。然而，规则越严格，结果值的随机性就越小。在极端情况下，非常严格的规则会使序列过度紧张，以至于有可能预测下一个数字，这将首先破坏使用随机数的目的。

此时，你应该问自己，过滤随机数的任何规则是否会严重损害随机性的数学完整性。明确回答这个问题的唯一方法是对过滤后的随机数进行基准测试，以衡量随机性的质量。开源程序 ENT 将运行各种指标来评估随机性，因此，如果你设计自己的规则，建议运行这些基准测试[Walker 08]。一般来说，只要规则不过度约束或预先确定下一个数字，就像给出的例子一样，那么随机性将适用于游戏 AI 中的几乎所有用途[Rabin 04]。

### 3.3.4 过滤随机性的实施细节

在实现滤波随机性时，必须注意将算法应用于随机性的每个特定用途。使用滤波随机性的每个唯一随机决策都需要保留自己的历史记录，以便从中过滤后续的随机数。否则，由于特定用途的数字序列在整个序列中不是连续的，尽管你进行了过滤，但纯粹的随机性可能会重新出现。例如，如果你需要玩家随机命中一个关键点的机会，那么这个序列必须与敌人的随机命中机会分开过滤。这两种用途是独立的，因此一种用途的严重打击不应影响另一种用途随后发生严重打击的机会。这样做的一个优点是，如果你愿意，你可以根据不同的用途改变你的过滤器特性。例如，当你的角色玩扑克或进行一些击打时，你可能想允许更多的序列出现，在这种情况下，“连胜”的感觉可能会有利于玩家的体验，但仍然会强烈限制关键命中的序列。

## 3.4 技术 3：游戏 AI 的 Perlin 噪声

如果你熟悉计算机图形学，你可能听说过 Perlin 噪声[Perlin 85，Perlin 02]。这种计算机生成的视觉效果是由肯·佩林于1983年开发的，由于其在电影数字效果中的广泛应用，他偶然获得了奥斯卡奖。Perlin 噪声通常用作生成有机纹理和几何体的组件。图 3.3 显示了 Perlin 噪声纹理的典型示例。

图 3.3
Perlin 噪声生成的具有不同细节级别的纹理的三个示例。

虽然 Perlin 噪声可用于帮助为视觉效果提供有机感（例如，烟雾、火或云的程序生成），但如何将这种技术用于游戏人工智能还不清楚。关键的认识是，Perlin 噪声产生了一种不均匀或正常的随机性，而是可以被描述为相干随机性，其中连续的随机数相互关联。随机性的这种“平滑”性质意味着我们不会从一个随机数到另一个随机数来疯狂跳跃，这可能是一个非常理想的特性。但这对游戏AI有什么用处呢？

第一步是在一维中可视化 Perlin 噪声，如图 3.4 所示。这可以被认为是一个随机游走信号（一系列相关的随机数）。我们可以使用这个信号来控制人工智能角色的特定行为特征随时间的变化。以下是游戏AI的可能性列表。

图 3.4
一维 Perlin 噪声。这是一系列随时间平滑漂移的随机数（相干随机性）。我们可以通过操纵产生数字的算法来控制外观和感觉。

- 运动（方向、速度、加速度）
- 分层到动画上（为面部运动或凝视添加噪声[Perlin 97]）
- 准确性（连胜或连败、处于最佳状态、运气或成功）
- 注意力（警卫警觉性、反应时间）
- 比赛风格（防守、进攻）
- 情绪（平静、愤怒、快乐、悲伤、抑郁、躁狂、无聊、订婚）

因此，虽然均匀或高斯随机性可用于改变人群中个体的物理或行为特征，但 Perlin 噪声可用于随时间变化这些特征。当你有大量的角色时，这可以使模拟更加有趣，因为每个特定的角色都可以在几秒钟、几分钟或几小时的持续时间内改变他们的行为特征。

让我们更深入地探讨一下前面列出的可能的游戏AI用途中的一些例子。通过随时间改变转向方向和速度，可以实现简单的漫游。对于任意漂移，Perlin 噪声是 Craig Reynolds 提出的用于转向行为的自组织相干随机性的更好替代方案，因为 Perlin 噪声比 Reynold 的解决方案更具可配置性[Reynolds 99]。

冷热条纹可以有目的地模拟，而不是在事后意外发生。这尤其有用，因为你可以预测连胜，并可以在中途让玩家意识到它，也许可以说“伙计们，我今晚感觉很幸运！”虽然均匀的随机性会有自然的连胜，但 Perlin 噪声可以配置为控制连胜的行为，并预测你什么时候会进入连胜。

另一个可能的用途是以某种方式改变角色的情绪，例如，按照平静与愤怒的等级。虽然你可以想出一个底层模拟来生成这些特征或情绪变化中的任何一个，但使用 Perlin 噪声随机生成它们可能会矫枉过正，而且更简单（在开发时间和计算方面），尤其是在玩家没有仔细检查任何一个人的情况下。然而，玩家可能会将注意力集中在一个角色上，在这种情况下，行为偏差背后明显缺乏合理性可能会造成问题。这个问题的一个解决方案是，当确定角色被注意到时，将模拟从 Perlin 噪声转移到更稳健的噪声。从这个意义上说，Perlin 噪声可以是一个 LOD 级别，当它被认为可能会导致现实中的中断时，可以被取代[Shine Hill 13a]。此外，如果角色现在正被玩家观看，那么可能是时候为 AI 创建一个不在场证明或背景故事了，正如本书中的另一篇文章[Shine Hill 13b]所述。

### 3.4.1 生成一维 Perlin 噪声

现在我们知道为什么需要一维 Perlin 噪声，让我们看看如何生成它并制作输出以满足我们的需求。在我们描述算法时，请注意旋钮和控件，它们将允许您根据自己的喜好自定义随机性。这些对于充分利用算法至关重要。当你探索生成部分时，你可能想在这本书的网站上获得演示，并尝试不同的设置。

虽然由于数学的细微差别，Perlin 噪声的产生很难解释，但我们将通过使用数字为您提供更直观的视觉解释。请注意，确切的细节可以在演示中的示例代码中看到。

在一维中，Perlin 噪声是通过首先决定使用多少个八度音阶来构建的。每个八度音阶都以特定的比例对信号细节做出贡献，更高的八度音阶会增加更精细的细节。每个八度音阶都是单独计算的，然后将它们相加以产生最终信号。图 3.5 显示了由四个倍频程构成的一维 Perlin 噪声。

为了解释每个八度音阶信号是如何产生的，让我们从第一个开始。第一个八度音阶是通过在[0,1]范围内用两个不同的均匀随机数开始和结束音程来计算的。中间的信号是通过应用在两者之间进行插值的数学函数来计算的。使用的理想函数是S曲线函数 6t^5^-15t^4^+10t^3^，因为它具有许多很好的数学性质，例如在一阶和二阶导数中是平滑的[Perlin 02]。这是理想的，因此包含在较高倍频程内的信号是平滑的。

对于第二个八度音阶，我们选择三个均匀的随机数，将它们彼此等距放置，然后使用我们的 sigmoid 函数在它们之间插值。同样，对于第三个八度音阶，我们选择五个均匀的随机数，将它们彼此等距放置，然后在它们之间插值。给定八度音阶的均匀随机数等于 2^(n-1)^+1。图 3.5 显示了四个八度音阶，每个八度音阶内都有随机选择的数字。

一旦我们有了八度音阶，下一步就是用振幅缩放每个八度音阶。这将导致较高的八度音阶逐渐对最终信号中的细粒度方差产生影响。从第一个八度音阶开始，我们将信号乘以 0.5 的振幅，如图 3.5 所示。第二个倍频程乘以 0.25 的振幅，第三个倍频程乘 0.125 的振幅，以此类推。给定倍频程的振幅公式为 p^i^，其中 p 是持续值，i 是倍频程（我们的例子使用了 0.5 的持续值）。持久性值将控制较高倍频程的影响程度，高持久性值会赋予较高倍频程更多的权重（在最终信号中产生更多的高频噪声）。

现在八度音阶已经适当缩放，我们可以将它们加在一起，得到最终的一维 Perlin 噪声信号，如图 3.5 右下角所示。虽然这一切都很好，但重要的是要意识到，就游戏 AI 而言，你不会计算和存储整个最终信号，因为不需要一次拥有整个信号。相反，给定沿信号的特定时间，在 x 轴的 [0,1] 范围内，您只需根据模拟需要计算该特定点。因此，如果你想把点放在最终信号的中间，你可以在 0.5 的时间计算每个倍频程中的单个信号，用它们正确的幅度缩放每个倍频程的值，然后把它们加在一起得到一个值。然后，您可以以任何速度运行模拟，例如请求 0.500001、0.51 或 0.6 的下一个点。

图 3.5
生成具有四个倍频程的一维 Perlin 噪声。

#### 3.4.1.1 控制 Perlin 噪声

如前一节所述，有几个控件可以让您自定义噪声的随机性。以下列表为摘要。

- 倍频程数量：较低的倍频程提供更大的信号摆动，而较高的倍频程则提供更细粒度的噪声。这也可以在人群中随机化，这样一些人在产生特定行为特征时比其他人有更多的八度音阶。
- 八度音阶范围：您可以有任何范围，例如八度音阶4到8。你不必从八度音阶1开始。同样，范围可以在人群中随机化。
- 每个倍频程的振幅：每个倍频程振幅的选择可用于控制最终信号。振幅越高，倍频程对最终信号的影响就越大。如果你不希望最终信号超过1.0，只需确保所有倍频程的振幅之和不超过1.0。
- 插值选择：S 曲线函数通常用于 Perlin 噪声，原始 Perlin 噪声使用3t^2-2t^3[Perlin 85]，改进的 Perlin 噪声则使用 6t^5^-15t^4^+10t^3^（二阶导数平滑）[Perlin 02]。然而，通过选择不同的公式[Komppa 10]，您可能会获得其他有趣的效果。

#### 3.4.1.2 超出区间的 Perlin 噪声采样

在查看图 3.5 时，您可能会想到，一旦我们到达间隔的末尾（信号的最右侧边缘），就会出现问题。一旦我们在时间 1.0 采样，由于信号突然停止，就没有地方可去了。一种解决方案是在时间 0.0 重新开始，但这会导致巨大的不连续性和重复行为。幸运的是，有一个更优雅的解决方案。

当我们需要在时间 1.0 之后进行采样时，我们可以生成一个全新的 Perlin 噪声信号，该信号附着在当前信号的末尾。如果我们简单地将每个八度音阶右边缘的所有均匀随机数复制到每个新八度音阶的最左边缘，那么我们的信号将无缝迁移到新的 Perlin 噪声信号中（过渡的平滑度取决于所使用的特定插值函数）。当然，我们需要为每个新倍频程内的剩余时隙新生成的均匀随机数，但这是可取的，这样我们就可以开始生成一个全新的信号。

### 3.4.2 游戏 AI Perlin Noise 演示

在书的网站上(http://www.gameaipro.com)，您将找到本节附带的Perlin噪声演示。在演示中，您可以使用各种旋钮来控制噪音的产生。Perlin噪声用于改变单个代理的漫游、速度和攻击性。此外，查看源代码以发现Perlin噪声生成的确切细节。

## 3.5 结论

本文介绍了三种先进的随机性技术，有助于增强我们的老朋友 rand() 函数。虽然均匀随机性是游戏变化的支柱，但高斯随机性、滤波随机性和 Perlin 噪声等技术可以提供普通老 rand() 本身无法提供的酷炫技巧。

在追求现实主义的过程中，高斯随机性为我们提供了正态分布，有助于模拟现实生活中我们周围的实际变化，无论是身体特征、认知能力、反应时间还是子弹传播的自然变化。为了保持玩家的内容，我们可以利用过滤后的随机性来确保对玩家产生积极或消极影响的有影响力的随机决策看起来公平公正。最后，Perlin 噪声不再只是用于图形。Perlin 噪声的一维相干随机性可用于随时间平滑地改变运动、动画和数十种其他行为特征。

最后，这三种技术都有附带的代码和演示，可以在本书的网站上找到(http://www.gameaipro.com).



# 第 II 部分 架构（Architecture）





# 第 VII 部分 零星杂项（Odds and Ends）

# 45 介绍 AI 的 GPGPU

*Conan Bourke and Tomasz Bednarz*

## 45.1 引言

在过去的十年里，计算机硬件已经取得了长足的进步。一个核心，两个核心，四个核心，现在成百上千个核心！计算机的功能已经从 CPU 转移到 GPU，新的 API 允许程序员控制这些芯片，而不仅仅是图形处理。

随着硬件的每一次进步，游戏系统都获得了越来越多的处理器能力，从而提供了比以前想象的更复杂、更详细的体验。对于人工智能来说，这意味着更现实的代理能够与彼此、玩家和周围环境进行更复杂的交互。

硬件方面最重要的进步之一是 GPU，它从纯粹的渲染处理器转变为能够在一定范围内进行任何计算的通用浮点处理器。AMD 和 NVIDIA 这两家主要的硬件供应商都生产通常具有 512 个以上处理器的 GPU，最新型号在消费类型号中提供了约 3000 个处理器，每个处理器都能够并行处理数据。这是一个很大的处理能力，我们不必将其全部用于高端图形。即使是 Xbox 360 也有一个能够进行基本通用处理技术的 GPU，而索尼的 PS3 的架构类似于能够进行相同处理的 GPU。

这些处理器的一个常见缺点是 CPU 和 GPU 之间的延迟和带宽，但在将 GPU 的线性计算模型与 CPU 的通用处理模型（称为加速处理单元（APU））相结合方面，尤其是 AMD，正在取得进展，这大大降低了这种延迟，并提供了本章稍后将讨论的其他优势。

## 45.2 GPGPU 的历史

GPU 上的通用计算（GPGPU）是指使用 GPU 进行渲染以外的计算[Harris 02]。随着早期着色器模型的引入，程序员能够修改 GPU 处理的方式和内容，不再依赖 OpenGL 或 Direct3D 的固定功能管道，也不必被迫渲染图像以供查看。相反，纹理可以用作数据缓冲区，在像素片段中访问和处理，结果可以绘制到输出纹理中。这种方法的缺点是缓冲区是只读的或只写的。此外，元素独立性的限制以及在图形算法和渲染管道的背景下表示问题的需要使这种技术变得繁琐。尽管如此，着色器已经从简单的汇编语言程序演变为几种新的类 C 语言，硬件供应商开始承认 GPGPU 领域的增长。

已经完成了在渲染管线之外暴露 GPU 功能的工作，因此创建了 API，让程序员可以访问 GPU 的功能，而不必将其视为纯粹的基于图形的设备，这些 API 带来了具有读写修改权限和额外数学功能的缓冲区。

2007年2月，NVIDIA 推出了计算统一设备架构（CUDA）API 和 G80 系列 GPU，这大大加速了 GPGPU 领域，并与 DirectX11 一起发布了 DirectCompute API。它与 CUDA 有许多相似的想法和方法，但具有使用 DirectX 现有资源的优势，允许在 DirectCompute 和 Direct3D 之间轻松共享数据以实现可视化。DirectCompute 也可以在 DirectX10 硬件上使用，但仍然受到 DirectX 其他部分的限制，因为它只能在基于 Windows 的系统上运行。

与之竞争的标准 OpenCL（开放计算语言）于 2008 年首次发布。OpenCL 使开发人员能够轻松地为异构架构（如多核 CPU 和 GPU，甚至索尼的 PS3）编写高效的跨平台应用程序，该应用程序具有基于现代 C 语言的单一编程接口。OpenCL 的规范由 Khronos Group[Khronos] 管理，他们还提供了一组我们在本文中使用的 C++ 绑定。这些绑定极大地简化了主机 API 的设置和代码开发速度。

## 45.3 OpenCL

OpenCL 程序被编写为“内核”，即在单个处理单元（计算单元）上与其他处理单元并行执行的函数，彼此独立工作。内核代码与 C 代码非常相似，并支持许多内置的数学函数。

当宿主程序运行时，我们需要执行以下步骤来使用 OpenCL：

1. OpenCL 枚举系统中可用的平台和计算设备，即所有 CPU 和 GPU。在每个设备内都有一个或多个计算单元，在这些处理单元内处理实际计算。单元的数量对应于设备（CPU 或 GPU）可以同时执行的独立指令的数量。因此，具有 8 个单元的 CPU 仍然被视为单个设备，具有 448 个单元的 GPU 也是如此。
2. 选择最强大的设备（通常是 GPU），并设置其上下文以进行进一步操作。
3. 在主机应用程序和 OpenCL 之间配置数据共享。
4. 使用内核代码和 OpenCL 上下文为您的设备构建 OpenCL 程序，然后从编译的内核代码中提取内核对象。
5. OpenCL 利用命令队列来控制内核执行的同步。向内核读取数据和从内核写入数据以及操作内存对象也由命令队列执行。
6. 内核在所有处理单元中被调用和执行。并行线程共享内存并使用屏障进行同步。最终，工作项的输出被读回主机内存以供进一步操作。

## 45.4 简单群集（Simple Flocking）和 OpenCL

我们将简要介绍使用 OpenCL 将经典 AI 算法转换为在 GPU 上运行。Craig Reynolds[Reynolds 87]介绍了用于控制自主移动代理的转向行为的概念，以及用于模拟人群和羊群的群集和 boids 的概念。许多 RTS 游戏都利用了具有美丽效果的群集——Relic Entertainment 的《家园》系列就是一个这样的例子——但这些游戏通常在可用的代理数量上有限。将此算法转换为在 GPU 上运行，我们可以很容易地将代理数量增加到数千。

我们将在 CPU 和 GPU 上实现一种强力群集方法，以证明通过简单地切换到 GPU 而不使用任何分区，利用 GPU 大规模并行架构可以轻松获得收益。利用简单的空间划分方案，使用 PS3 的 Cell Architecture[Renods 06]进行了类似的工作。清单 45.1 给出了一个基本群集算法的伪代码，该算法使用优先加权力之和进行分离、内聚和对齐，还包括一个漫游行为来帮助随机化代理。首要任务是漫游，然后是分离、凝聚力，最后是对齐。所有速度在应用于位置之前必须更新，否则初始试剂会错误地影响后续试剂。

Listing 45.1 群集伪代码

```
for each agent
    for each neighbor within radius
         calculate agent separation force away from neighbor
         calculate center of mass for agent cohesion force
         calculate average heading for agent alignment force
    calculate wander force
    sum prioritized weighted forces and apply to agent velocity
for each agent
	apply velocity to position
```

在 CPU 上实现该算法很简单。通常，一个代理将由一个包含相关代理信息（即位置、速度、漫游目标等）的对象组成。然后，一系列代理将被循环两次；首先更新每个代理的力和速度，然后应用新的速度。

在将此算法转换为 GPU 时，必须考虑一些因素，但将 CPU 代码转换为 OpenCL 代码很简单。如上面的伪代码所示，为每个代理计算所有邻居，每个代理的复杂度为 O(n^2）。在 CPU 上，这是通过所有代理的双循环来实现的。在 GPU 上，我们能够并行化外循环，并顺序执行每个工作项（每个代理）的内环交互，从而大大缩短了处理时间。

可以实现空间分区技术来提高性能，但必须注意的是，GPU 以非常线性的方式工作，非常适合处理数据阵列，这通常是在图形处理的顶点阵列的情况下完成的。在复杂的空间分区方案（如八叉树）的情况下，GPU 在尝试访问非线性内存时会抖动。Craig Reynolds 对 PS3 的解决方案是使用一个简单的三维桶网格来存储相邻的代理[Reynolds 06]。这允许对桶进行线性处理，代理只需对与其直接相邻的桶具有读取权限。然而，在本文中，我们演示了一个从 CPU 到 GPU 的简单转换，而没有进行这种优化，以展示转换为 GPGPU 处理的即时收益。

转换为 GPGPU 的第一步是将数据分解为连续的数组。GPU最多可以处理三维数组，但在我们的示例中，我们将把代理分解为代理中每个元素的一维数组，即位置、速度等。

同样值得注意的是，在 OpenCL 术语中有两种类型的内存：局部和全局。区别在于，全局内存可以由任何内核访问，而本地内存是进程独有的，因此访问速度要快得多。把它想象成 RAM 和 CPU 的缓存。

## 45.5 OpenCL 设置

使用C++主机绑定可以直接初始化计算设备。首先，必须枚举主机平台才能访问底层计算设备。然后，从平台创建上下文（请注意，在这个例子中，我们使用 CLD_TYPE_GPU 初始化上下文以专门使用 GPU）以及命令队列，以便通过上下文执行计算内核和排队内存传输。有关详细信息，请参阅清单 45.2。

Listing 45.2 OpenCL host setup.

```c++
cl::Platform::get(&m_oclPlatforms);
cl_context_properties aoProperties[] = {
    CL_CONTEXT_PLATFORM,
    (cl_context_properties)(m_oclPlatforms[0])(),
    0};
m_oclContext = cl::Context(CL_DEVICE_TYPE_GPU, aoProperties);
m_oclDevices = m_oclContext.getInfo<CL_CONTEXT_DEVICES>();
std::cout << “OpenCL device count: “ << m_oclDevices.size();
m_oclQueue = cl::CommandQueue(m_oclContext, m_oclDevices[0]);
```

OpenCL 有两种类型的内存对象：缓冲区和图像。缓冲区包含使用单指令多数据（SIMD）处理模型的标准 4D 浮点向量，而图像是根据纹理像素定义的。为了本文的目的，缓冲区被选择为更适合表示彼此相邻的代理。

缓冲区可以初始化为只读、只写或读写，如清单 45.3 所示。创建缓冲区是为了容纳模拟将使用的最大数量的代理，尽管如果我们愿意，我们可以处理更少的代理。除了代理数据，我们还向内核发送群集算法的参数，以及一个时间值，该时间值指定了自上一帧以来一致速度的经过时间。

Listing 45.3 OpenCL buffer setup

```c++
typedef struct Params
{
    float fNeighborRadiusSqr;
    float fMaxSteeringForce;
    float fMaxBoidSpeed;
    float fWanderRadius;
    float fWanderJitter;
    float fWanderDistance;
    float fSeparationWeight;
    float fCohesionWeight;
    float fAlignmentWeight;
    float fDeltaTime;
} Params;
cl::Buffer m_clVPosition;
cl::Buffer m_clVVelocity;
cl::Buffer m_clVParams;
...
m_clVPosition = cl::Buffer(m_oclContext, CL_MEM_READ_WRITE, 
uiMaxAgentCount * 4 * sizeof(float));
m_clVParams = cl::Buffer(m_oclContext, CL_MEM_READ_ONLY, 
sizeof(Params));
```

为了创建计算内核，我们需要将内核代码编译成 CL 程序，然后提取计算内核。在我们的示例中，内核代码位于一个单独的文件 program.cl 中，加载该文件以创建程序，如清单 45.4 所示。

Listing 45.4 Building the OpenCL program

```c++
//read source file
std::ifstream sFile(“program.cl”);
std::string sCode(std::istreambuf_iterator<char>(sFile),
	(std::istreambuf_iterator<char>()));
cl::Program::Sources oSource(1,
	std::make_pair(sCode.c_str(), sCode.length() + 1));
//build the program for the specified devices
m_oclProgram = cl::Program(m_oclContext, oSource);
m_oclProgram.build(m_oclDevices);
m_clKernel = cl::Kernel(m_oclProgram, “Flocking”);
```

清单 45.5 显示了我们示例内核的一部分，省略了主体，因为它与 CPU 实现几乎相同。然而，值得注意的是内核中关于屏障的最后一部分。在 CPU 上，我们循环两次，在计算完所有代理后对其施加力。我们可以在内核中通过设置一个屏障来实现这一点，这会导致所有执行的线程在此时等待，直到所有线程都赶上。在内核中，我们可以根据缓冲区的大小，使用 0、1 或 2 调用 `get_global_id(0)` 来访问输入缓冲区的当前索引。

Listing 45.5 The OpenCL kernel

```c++
__kernel void Flocking(
     __global float4* vPosition,
     ...
     __constant struct Params* pp)
{
    //get_global_id(0) accesses the current element index
    unsigned int i = get_global_id(0);
    ...
    barrier(CLK_LOCAL_MEM_FENCE | CLK_GLOBAL_MEM_FENCE);
    vPosition[i] + = vVelocity[i] * pp->fDeltaTime;
    barrier(CLK_LOCAL_MEM_FENCE | CLK_GLOBAL_MEM_FENCE);
}
```

一旦构建了内核，内核参数就会显式传递给 OpenCL，如清单 45.6 所示。在执行内核时，必须将参数预加载到相应的参数索引中，而不是将参数传递给内核。

Listing 45.6 Specifying kernel arguments

```c++
m_clKernel.setArg(0, sizeof(cl_mem), &m_clVPosition);
m_clKernel.setArg(1, sizeof(cl_mem), &m_clVVelocity);
```

一旦所有内容都初始化并构建完成，我们就可以将内核排队进行计算。内核不会立即执行，而是排队等待处理。启动内核时，全局工作大小必须等于要处理的元素数量。我们还可以在数组范围内指定偏移量，但我们可以指定一个从数组前面开始的 NullRange。请参考清单 45.7。

Listing 45.7 Executing the kernel program

```c++
m_oclQueue.enqueueNDRangeKernel(
    m_clKernel, cl::NullRange,
    cl::NDRange(uiMaxAgentCount),
    cl::NullRange,
    nullptr, nullptr);
```

## 45.6 系统间共享GPU处理

开发人员在使用 GPGPU 时，特别是对于游戏开发人员来说，最初的担忧可能是图形处理时间被占用了。当今的许多高端游戏都使用 GPGPU 进行图形预处理和后处理，也使用 NVidia 的 PhysX 等 API 进行物理模拟。将 AI 添加到组合中将减少这些其他系统可用的处理时间。这是一个无法避免的问题。然而，GPU 处理能力已经有了巨大的飞跃，从英伟达 500 系列的数百个内核，到 600 系列的数千个内核。随着时间的推移，更多的系统将拥有更多的处理能力，开发人员将开始发现这种能力除了图形、物理和人工智能之外的其他有趣用途。

与此同时，至少对于 OpenCL，存在互操作性 API，允许在 OpenGL 和 Direct3D 之间共享 OpenCL 缓冲区，从而减少了不断将信息传输到 GPU 并返回 CPU 的需要。AI 代理的位置缓冲区既可以用于 GPU 上的群集计算，也可以用于渲染代理的硬件实例化的渲染缓冲区，而不需要将数据返回给 CPU，只需将其传输回 GPU 即可。

## 45.7 结果

图 45.1 显示了使用我们示例群集算法的暴力实现处理代理的性能（以毫秒为单位）（越低越好）。如图所示，GPU 通过更高的代理数量提供了巨大的性能提升，将算法转换为 OpenCL 所需的工作量最小。然而，由于缓冲区转换，在较低的代理计数下，GPU 的运行速度比 CPU 慢，如图 45.2 所示。还测试了用于计算和研究的 GPU，以显示此类设备中优化的带宽和延迟，从而比消费者级 GPU 更快地进行计算，同时也可以洞察未来消费者级的性能。

图45.1

使用各种 GPU 和 CPU 处理 512、1024、4096、8192 和 16384 个代理计数的性能（以毫秒为单位）。

图45.2

代理计数较低时的性能（毫秒）。消费者 GPU 保持不变，因为代理的处理几乎不需要任何时间，但 GPU 之间的缓冲区传输没有得到优化。然而，C2050 的带宽和延迟要高得多

## 45.8 结论

正如我们的例子所示，我们可以很容易地将 GPGPU 计算用于使用极高代理数的游戏，例如使用数千个实体的 RTS游戏，而不是大多数当前 RTS 游戏中的标准数十到数百个实体。我们仍然很难将代理的决策转移到 GPU 上，但运动甚至避障等元素可以从 CPU 上转移出来。占用其他系统（如图形）的处理时间将是另一个问题，但可以通过各种互操作性 API 在一定程度上缓解。

人工智能 GPGPU 的其他例子也存在，神经网络和寻路小组已经完成了工作，经典的康威生命游戏可以很容易地在 GPU 上实现[Rumpf 10]。GPGPU 中可用的 AI 处理类型的主要限制是大多数 AI 决策的分支性质。

APU 可以让我们将决策技术与 GPGPU 技术紧密结合，但消费者对此类设备的接受程度将决定这种 AI 风格是否会更多地出现在游戏中。

目前，GPGPU 是大规模模拟的一个可行选择，根据 Valve 的硬件调查，在撰写本文时，目前最常见的消费级 GPU 是装有 336 个处理器的英伟达 GTX 560。这足以满足我们的人工智能需求。