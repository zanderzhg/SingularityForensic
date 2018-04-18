﻿using CDFC.Util;
using SingularityForensic.Contracts.App;
using SingularityForensic.Contracts.Hash;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SingularityForensic.Hash {
    [Export(typeof(IHasher))]
    public class MD5Hasher : HasherBase {
        public MD5Hasher():base(MD5HashAlgorithmProvider.StaticInstance) {
            
        }
        public override string HashTypeName => LanguageService.FindResourceString(Constants.HashTypeName_MD5);

        public override string GUID => Constants.HashTypeGUID_MD5;

        public override int Sort => 2;

        class MD5HashAlgorithmProvider : GenericStaticInstance<MD5HashAlgorithmProvider>,IHashAlgorithmProvider {
            public HashAlgorithm CreateNew() => new MD5CryptoServiceProvider();
        }
    }
}