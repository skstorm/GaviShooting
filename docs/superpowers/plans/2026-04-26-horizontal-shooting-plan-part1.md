# 횡스크롤 슈팅 게임 구현 계획

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 미니멀 횡스크롤 슈팅 프로토타입 — 스테이지 1개, 적 2~3종, 보스 1마리

**Architecture:** Logic-View 분리, 고정 프레임 루프, 커맨드 패턴 기반. game-programmer 에이전트의 설계 사상/코딩룰 준수. 게임 주도 점진적 구축.

**Tech Stack:** Unity 6, URP 2D, Input System, UniTask, NUnit (EditMode Test)

**Spec:** `docs/superpowers/specs/2026-04-26-horizontal-shooting-design.md`
**Agent:** `.claude/agents/game-programmer.md`

---

## Task 1: UniTask 패키지 설치

**Files:**
- Modify: `Packages/manifest.json`

- [ ] **Step 1: manifest.json에 UniTask 추가**

`Packages/manifest.json`의 `dependencies`에 추가:
```json
"com.cysharp.unitask": "https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask"
```

- [ ] **Step 2: 커밋**
```bash
git add Packages/manifest.json
git commit -m "add: UniTask 패키지 추가"
```

---

## Task 2: Core — FrameManager + 테스트

**Files:**
- Create: `Assets/Scripts/Core/FrameManager.cs`
- Create: `Assets/Tests/EditMode/Core/FrameManagerTest.cs`
- Create: `Assets/Tests/EditMode/EditModeTests.asmdef`
- Create: `Assets/Scripts/Core/GaviShooting.Core.asmdef`

- [ ] **Step 1: Core asmdef 생성**

Create `Assets/Scripts/Core/GaviShooting.Core.asmdef`:
```json
{
    "name": "GaviShooting.Core",
    "rootNamespace": "GaviShooting.Core",
    "references": [],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": false,
    "precompiledReferences": [],
    "autoReferenced": true,
    "defineConstraints": [],
    "versionDefines": [],
    "noEngineReferences": true
}
```

- [ ] **Step 2: EditMode 테스트 asmdef 생성**

Create `Assets/Tests/EditMode/EditModeTests.asmdef`:
```json
{
    "name": "EditModeTests",
    "rootNamespace": "",
    "references": ["GaviShooting.Core", "GaviShooting.Logic"],
    "includePlatforms": ["Editor"],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": true,
    "precompiledReferences": ["nunit.framework.dll"],
    "autoReferenced": false,
    "defineConstraints": ["UNITY_INCLUDE_TESTS"],
    "versionDefines": [],
    "noEngineReferences": false
}
```

- [ ] **Step 3: FrameManager 실패 테스트 작성**

Create `Assets/Tests/EditMode/Core/FrameManagerTest.cs`:
```csharp
using NUnit.Framework;
using GaviShooting.Core;

namespace Tests.EditMode.Core
{
    public class FrameManagerTest
    {
        [Test]
        public void CalcDeltaFrame_FirstCall_ReturnsOne()
        {
            var fm = new FrameManager(60);
            int delta = fm.CalcDeltaFrame(1f / 60f);
            Assert.AreEqual(1, delta);
        }

        [Test]
        public void CalcDeltaFrame_HalfFrame_ReturnsZero()
        {
            var fm = new FrameManager(60);
            int delta = fm.CalcDeltaFrame(1f / 120f);
            Assert.AreEqual(0, delta);
        }

        [Test]
        public void CalcDeltaFrame_AccumulatesTwoHalves_ReturnsOne()
        {
            var fm = new FrameManager(60);
            fm.CalcDeltaFrame(1f / 120f);
            int delta = fm.CalcDeltaFrame(1f / 120f);
            Assert.AreEqual(1, delta);
        }

        [Test]
        public void CalcDeltaFrame_LargeStep_ReturnsMultiple()
        {
            var fm = new FrameManager(60);
            int delta = fm.CalcDeltaFrame(3f / 60f);
            Assert.AreEqual(3, delta);
        }
    }
}
```

- [ ] **Step 4: FrameManager 구현**

Create `Assets/Scripts/Core/FrameManager.cs`:
```csharp
namespace GaviShooting.Core
{
    public class FrameManager
    {
        private readonly float _secPerFrame;
        private float _accumulator;

        public FrameManager(int fps)
        {
            _secPerFrame = 1f / fps;
        }

        /// <summary>
        /// 경과시간 축적 방식으로 계산하여 가변 프레임레이트에서도 일정한 논리 프레임 수를 보장한다
        /// </summary>
        public int CalcDeltaFrame(float deltaTime)
        {
            _accumulator += deltaTime;
            int frames = (int)(_accumulator / _secPerFrame);
            _accumulator -= frames * _secPerFrame;
            return frames;
        }
    }
}
```

- [ ] **Step 5: 테스트 실행 확인 후 커밋**
```bash
git add Assets/Scripts/Core/ Assets/Tests/
git commit -m "add: FrameManager — 고정 프레임 계산 + 테스트"
```

---

## Task 3: Core — CommandQueue + 테스트

**Files:**
- Create: `Assets/Scripts/Core/ICommand.cs`
- Create: `Assets/Scripts/Core/ICommandQueue.cs`
- Create: `Assets/Scripts/Core/CommandQueue.cs`
- Create: `Assets/Tests/EditMode/Core/CommandQueueTest.cs`

- [ ] **Step 1: ICommand, ICommandQueue 인터페이스 작성**

Create `Assets/Scripts/Core/ICommand.cs`:
```csharp
namespace GaviShooting.Core
{
    public interface ICommand
    {
        void Execute();
    }
}
```

Create `Assets/Scripts/Core/ICommandQueue.cs`:
```csharp
namespace GaviShooting.Core
{
    public interface ICommandQueue
    {
        void Enqueue(ICommand command);
        void ProcessAll();
        int Count { get; }
    }
}
```

- [ ] **Step 2: 실패 테스트 작성**

Create `Assets/Tests/EditMode/Core/CommandQueueTest.cs`:
```csharp
using NUnit.Framework;
using GaviShooting.Core;

namespace Tests.EditMode.Core
{
    public class CommandQueueTest
    {
        private class StubCommand : ICommand
        {
            public int ExecuteCount;
            public void Execute() => ExecuteCount++;
        }

        [Test]
        public void Enqueue_IncreasesCount()
        {
            var queue = new CommandQueue();
            queue.Enqueue(new StubCommand());
            Assert.AreEqual(1, queue.Count);
        }

        [Test]
        public void ProcessAll_ExecutesAllCommands()
        {
            var queue = new CommandQueue();
            var cmd1 = new StubCommand();
            var cmd2 = new StubCommand();
            queue.Enqueue(cmd1);
            queue.Enqueue(cmd2);

            queue.ProcessAll();

            Assert.AreEqual(1, cmd1.ExecuteCount);
            Assert.AreEqual(1, cmd2.ExecuteCount);
        }

        [Test]
        public void ProcessAll_ClearsQueue()
        {
            var queue = new CommandQueue();
            queue.Enqueue(new StubCommand());
            queue.ProcessAll();
            Assert.AreEqual(0, queue.Count);
        }

        [Test]
        public void ProcessAll_EmptyQueue_DoesNothing()
        {
            var queue = new CommandQueue();
            Assert.DoesNotThrow(() => queue.ProcessAll());
        }
    }
}
```

- [ ] **Step 3: CommandQueue 구현**

Create `Assets/Scripts/Core/CommandQueue.cs`:
```csharp
using System.Collections.Generic;

namespace GaviShooting.Core
{
    public class CommandQueue : ICommandQueue
    {
        private readonly Queue<ICommand> _queue = new();

        public int Count => _queue.Count;

        public void Enqueue(ICommand command)
        {
            _queue.Enqueue(command);
        }

        public void ProcessAll()
        {
            while (_queue.Count > 0)
            {
                _queue.Dequeue().Execute();
            }
        }
    }
}
```

- [ ] **Step 4: 테스트 실행 확인 후 커밋**
```bash
git add Assets/Scripts/Core/ Assets/Tests/
git commit -m "add: CommandQueue — 커맨드 큐 + 테스트"
```
