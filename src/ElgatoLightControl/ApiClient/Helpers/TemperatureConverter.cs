namespace ElgatoLightControl.ApiClient.Helpers;

internal static class TemperatureConverter
{
	private static readonly Lazy<Dictionary<ushort, ushort>> KelvinToMiredsMap = new(() =>
		new Dictionary<ushort, ushort>
		{
			{ 7000, 143 },
			{ 6950, 144 },
			{ 6900, 145 },
			{ 6850, 146 },
			{ 6800, 147 },
			{ 6750, 148 },
			{ 6700, 149 },
			{ 6650, 150 },
			{ 6600, 151 },
			{ 6550, 152 },
			{ 6500, 153 },
			{ 6450, 155 },
			{ 6400, 156 },
			{ 6350, 157 },
			{ 6300, 158 },
			{ 6250, 160 },
			{ 6200, 161 },
			{ 6150, 162 },
			{ 6100, 163 },
			{ 6050, 165 },
			{ 6000, 167 },
			{ 5950, 168 },
			{ 5900, 169 },
			{ 5850, 171 },
			{ 5800, 172 },
			{ 5750, 174 },
			{ 5700, 175 },
			{ 5650, 177 },
			{ 5600, 179 },
			{ 5550, 180 },
			{ 5500, 182 },
			{ 5450, 183 },
			{ 5400, 185 },
			{ 5350, 187 },
			{ 5300, 189 },
			{ 5250, 190 },
			{ 5200, 192 },
			{ 5150, 194 },
			{ 5100, 196 },
			{ 5050, 198 },
			{ 5000, 200 },
			{ 4950, 202 },
			{ 4900, 204 },
			{ 4850, 206 },
			{ 4800, 208 },
			{ 4750, 211 },
			{ 4700, 213 },
			{ 4650, 215 },
			{ 4600, 217 },
			{ 4550, 220 },
			{ 4500, 222 },
			{ 4450, 225 },
			{ 4400, 227 },
			{ 4350, 230 },
			{ 4300, 233 },
			{ 4250, 235 },
			{ 4200, 238 },
			{ 4150, 241 },
			{ 4100, 244 },
			{ 4050, 247 },
			{ 4000, 250 },
			{ 3950, 253 },
			{ 3900, 256 },
			{ 3850, 260 },
			{ 3800, 263 },
			{ 3750, 267 },
			{ 3700, 270 },
			{ 3650, 274 },
			{ 3600, 278 },
			{ 3550, 282 },
			{ 3500, 286 },
			{ 3450, 290 },
			{ 3400, 294 },
			{ 3350, 299 },
			{ 3300, 303 },
			{ 3250, 308 },
			{ 3200, 313 },
			{ 3150, 317 },
			{ 3100, 323 },
			{ 3050, 328 },
			{ 3000, 333 },
			{ 2950, 339 },
			{ 2900, 345 }
		});

	private static readonly Lazy<Dictionary<ushort, ushort>> MiredsToKelvinMap = new(() =>
		KelvinToMiredsMap.Value.ToDictionary(kvp => kvp.Value, kvp => kvp.Key));

	internal static ushort MiredsToKelvin(ushort mireds)
	{
		if (mireds is < 143 or > 344)
		{
			throw new ArgumentOutOfRangeException(nameof(mireds), "Mireds must be between 143 and 344");
		}

		if (MiredsToKelvinMap.Value.TryGetValue(mireds, out var kelvin))
		{
			return kelvin;
		}

		var closest = MiredsToKelvinMap.Value.Keys.MinBy(k => Math.Abs(k - mireds));

		return MiredsToKelvinMap.Value[closest];
	}

	internal static ushort KelvinToMireds(ushort kelvin)
	{
		if (kelvin is < 2900 or > 7000)
		{
			throw new ArgumentOutOfRangeException(nameof(kelvin), "Kelvin must be between 2900 and 7000");
		}

		if (KelvinToMiredsMap.Value.TryGetValue(kelvin, out var mireds))
		{
			return mireds;
		}

		var closest = KelvinToMiredsMap.Value.Keys.MinBy(k => Math.Abs(k - kelvin));

		return KelvinToMiredsMap.Value[closest];
	}
}
