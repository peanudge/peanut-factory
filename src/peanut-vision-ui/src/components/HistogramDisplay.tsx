import Box from "@mui/material/Box";
import Skeleton from "@mui/material/Skeleton";
import { useQuery } from "@tanstack/react-query";
import { getImageHistogram } from "../api/client";
import { queryKeys } from "../api/queryKeys";

interface Props {
  imageId: string;
}

const HEIGHT = 80;
const BINS = 256;

export default function HistogramDisplay({ imageId }: Props) {
  const { data, isLoading } = useQuery({
    queryKey: queryKeys.imageHistogram(imageId),
    queryFn: () => getImageHistogram(imageId),
    staleTime: Infinity,
  });

  if (isLoading) {
    return <Skeleton variant="rectangular" width="100%" height={HEIGHT} sx={{ borderRadius: 1 }} />;
  }

  if (!data) return null;

  const channels: { values: number[]; color: string; opacity: number }[] = [
    { values: data.red,   color: "#f44336", opacity: 0.55 },
    { values: data.green, color: "#4caf50", opacity: 0.55 },
    { values: data.blue,  color: "#2196f3", opacity: 0.55 },
  ];

  // Build SVG path: one polygon per channel
  function buildPath(values: number[]): string {
    const w = 100 / BINS;
    let d = `M 0 ${HEIGHT}`;
    for (let i = 0; i < BINS; i++) {
      const x = i * w;
      const barH = values[i] * HEIGHT;
      d += ` L ${x} ${HEIGHT - barH} L ${x + w} ${HEIGHT - barH}`;
    }
    d += ` L 100 ${HEIGHT} Z`;
    return d;
  }

  return (
    <Box sx={{ width: "100%", height: HEIGHT, position: "relative" }}>
      <svg
        viewBox={`0 0 100 ${HEIGHT}`}
        preserveAspectRatio="none"
        width="100%"
        height={HEIGHT}
        style={{ display: "block" }}
      >
        {channels.map((ch) => (
          <path
            key={ch.color}
            d={buildPath(ch.values)}
            fill={ch.color}
            fillOpacity={ch.opacity}
            stroke="none"
          />
        ))}
      </svg>
    </Box>
  );
}
