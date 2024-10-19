import GoodMorningBar from '@/app/_components/good-morning-bar';
import AiToolsBar from '@/app/_components/ai-tools-bar';

export default function Home() {
  return (
    <main className="px-[58px] py-[12px] flex flex-col gap-4">
      <GoodMorningBar />
      <AiToolsBar />
    </main>
  );
}