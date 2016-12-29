using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Diagnostics;
using Machine.VSTestAdapter.Discovery;

namespace Machine.VSTestAdapter.Helpers
{
    public static class SpecTestHelper
    {
        public static TestCase GetVSTestCaseFromMSpecTestCase(string source, MSpecTestCase mspecTestCase, bool disableFullTestNames, Uri testRunnerUri, Func<string, string, dynamic> traitCreator)
        {
            VisualStudioTestIdentifier vsTest = mspecTestCase.ToVisualStudioTestIdentifier();

            TestCase testCase = new TestCase(vsTest.FullyQualifiedName, testRunnerUri, source)
            {
                DisplayName = disableFullTestNames ? mspecTestCase.SpecificationDisplayName : $"{mspecTestCase.ContextDisplayName} it {mspecTestCase.SpecificationDisplayName}"
            };

            if (MSpecTestAdapter.UseTraits)
            {
                dynamic dynTestCase = testCase;
                dynamic classTrait = traitCreator(Strings.TRAIT_CLASS, mspecTestCase.ClassName);
                dynamic subjectTrait = traitCreator(Strings.TRAIT_SUBJECT, string.IsNullOrEmpty(mspecTestCase.Subject) ? Strings.TRAIT_SUBJECT_NOSUBJECT : mspecTestCase.Subject);

                dynTestCase.Traits.Add(classTrait);
                dynTestCase.Traits.Add(subjectTrait);

                if (mspecTestCase.Tags != null)
                {
                    foreach (var tag in mspecTestCase.Tags)
                    {
                        if (!string.IsNullOrEmpty(tag))
                        {
                            dynamic tagTrait = traitCreator(Strings.TRAIT_TAG, tag);
                            dynTestCase.Traits.Add(tagTrait);
                        }
                    }
                }
            }

            Debug.WriteLine(string.Format("TestCase {0}", (object)testCase.FullyQualifiedName));
            return testCase;
        }
    }

}