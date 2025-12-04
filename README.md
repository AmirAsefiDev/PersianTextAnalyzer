# PersianTextAnalyzer
PersianTextAnalyzer is an experimental and research-oriented project for detecting and correcting spelling mistakes in Persian text. The project includes two main approaches for spell checking:

1. NHunspell

An open-source tool for Persian spell checking.

Capable of detecting many simple spelling mistakes.

Limitations:

Struggles with more complex mistakes or colloquial text.

Requires proper dictionaries and configuration for better results.

2. Sapling.ai API

An intelligent online service for Persian spell checking.

Can detect many errors, even in long and conversational sentences, and provides suggested corrections.

Limitations:

Sometimes misdetects words due to whitespace or character combinations.

May provide less accurate suggestions for ambiguous colloquial mistakes.

Important Note: The indexes and sentences returned by the API should be carefully used to extract mistakes (using sentence and sentence_start is recommended).

Short Conclusion

NHunspell is suitable for quick, local Persian spell checking but has limitations.

Sapling provides higher detection accuracy and works better for long or conversational text, but requires an internet connection and an API key, and may occasionally miss certain cases.

Combining both tools could create a preliminary pipeline for Persian text correction.
