"use client";

import Image from 'next/image';
import { AiPoweredSearchButton, AskAiAssistantButton } from '@/app/_components/buttons';

interface Props {
  onActivateAi: () => void;
};

export default function AiToolsBar({ onActivateAi }: Props) {
  return (
    <div className="flex items-center justify-between">
      <div className="flex gap-4 items-center">
        <h1 className="text-[#002F3F] font-bold text-[32px]">Everyday view</h1>
        <Image src="/icon-arrow-down.svg" alt="" width={32} height={32} />
      </div>
      <div className="flex gap-2 items-center">
        <AiPoweredSearchButton />
        <AskAiAssistantButton onClick={ () => onActivateAi() } />
      </div>
    </div>
  );
}
