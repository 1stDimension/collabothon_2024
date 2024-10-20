'use client';

import GoodMorningBar from '@/app/_components/good-morning-bar';
import AiToolsBar from '@/app/_components/ai-tools-bar';
import { useState } from 'react';
import AiChatPanel from './_components/ai-chat';
import DndArea from './_components/dnd-area';
export default function Home() {
  const [ showAiPane, setShowAiPane ] = useState(false);
  const showAi = () => setShowAiPane(true);
  const hideAi = () => setShowAiPane(false);

  return (
    <main className="px-[58px] py-[12px] flex flex-col gap-4">
      <GoodMorningBar />
      <AiToolsBar onActivateAi={ () => showAi() } />
      <AiChatPanel active={ showAiPane } onDismiss={ () => hideAi() } />
      <DndArea />
    </main>
  );
}