﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Diagnostics;
using Shouldly;
using Xunit;

namespace FizzWare.NBuilder.Tests.Integration
{
    
    public class IssueTests
    {

        [Fact]
        public void Guid_ShouldNotChangeValueOfStaticMember()
        {
            var guid1 = Guid.Empty;
            new Guid();
            Builder<Guid>.CreateNew().Build();
            var guid4 = Guid.Empty;

            guid1.ShouldBe(guid4);
        }

    }
}
